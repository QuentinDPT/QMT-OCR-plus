#include <windows.h>
#include <vector>
#include <iostream>
#include <chrono>
#include <string>
#include <thread>
#include "Image.h"

#define PROCESS_CONSUMMER_RELEASED  0x10
#define PROCESS_CONSUMMER_WORKING   0x11
#define PROCESS_PRODUCER_RELEASED   0x20
#define PROCESS_PRODUCER_WORKING    0x21


// Optimisation possible :
//  - Alignement cache pour chaque canal

std::size_t getL1CacheLineSize() {
    DWORD bufferSize = 0;
    GetLogicalProcessorInformationEx(RelationCache, nullptr, &bufferSize);

    std::vector<uint8_t> buffer(bufferSize);
    SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX* info =
        reinterpret_cast<SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX*>(buffer.data());

    if (GetLogicalProcessorInformationEx(RelationCache, info, &bufferSize)) {
        BYTE* ptr = buffer.data();
        while (ptr < buffer.data() + bufferSize) {
            auto* cacheInfo = reinterpret_cast<SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX*>(ptr);
            if (cacheInfo->Relationship == RelationCache) {
                std::size_t lineSize = cacheInfo->Cache.LineSize;
                return lineSize;
            }
            ptr += cacheInfo->Size;
        }
    }
    return 64; // fallback par défaut si échec
}


int createMMF(LPCWSTR MMFID, size_t memorySize, void** outputMemoryPtr, HANDLE* outputHandleMap) {
    *outputHandleMap = CreateFileMapping(
        INVALID_HANDLE_VALUE,
        nullptr,
        PAGE_READWRITE,
        0,
        (DWORD)memorySize,
        MMFID
    );

    if (!*outputHandleMap) {
        std::cerr << "Erreur CreateFileMapping\n";
        return -1;
    }

    *outputMemoryPtr = MapViewOfFile(*outputHandleMap, FILE_MAP_ALL_ACCESS, 0, 0, memorySize);
    if (!*outputMemoryPtr) {
        std::cerr << "Erreur MapViewOfFile\n";
        CloseHandle(*outputHandleMap);
        return -1;
    }

    return 0;
}

void closeMMF(void* memoryPtr, HANDLE memoryHandle) {
    UnmapViewOfFile(memoryPtr);
    CloseHandle(memoryHandle);
}

imaging::Image** bindMMFSpace(uint8_t* mmfSpace, uint8_t ringSize, size_t imageMemorySize) {
    imaging::Image** result = (imaging::Image**)malloc(sizeof(void*) * ringSize);

    for (unsigned int i = 0; i < ringSize; i++) {
        result[i] = (imaging::Image*)(mmfSpace + (imageMemorySize * i));
    }

    return result;
}

bool SpinWaitLock(std::atomic<uint8_t>* variable, const unsigned int spin_wait_max_iteration, const std::chrono::nanoseconds timeout, const uint8_t target_to_wait, const uint8_t value_to_set) {
    auto target = target_to_wait;

    for (unsigned int i = 0; i < spin_wait_max_iteration; i++) {
        target = target_to_wait;
        if (variable->compare_exchange_strong(target, value_to_set))
        {
            return true;
        }
    }

    auto before = std::chrono::system_clock::now();

    do {
        std::this_thread::yield();
        target = target_to_wait;
        if (variable->compare_exchange_strong(target, value_to_set))
            return true;
    } while (std::chrono::system_clock::now() - before <= timeout);

    return false;
}

int main()
{
    std::cout << "Hello World!\n";

    LPCWSTR mmfNamespace = L"YEP";

    imaging::Image emptyImg;
    emptyImg.channel_type = imaging::ImagePixelFormat::RGB8;
    emptyImg.channel_size = 3;
    emptyImg.width = 1600;
    emptyImg.height = 900;
    emptyImg.data = nullptr;

    std::cout << "width \t\t: 0x" << std::hex << &emptyImg.width << std::endl;
    std::cout << "height \t\t: 0x" << std::hex << &emptyImg.height << std::endl;
    std::cout << "channel_type \t: 0x" << std::hex << &emptyImg.channel_type << std::endl;
    std::cout << "channel_size \t: 0x" << std::hex << (int*) &emptyImg.channel_size << std::endl;
    std::cout << "align \t\t: 0x" << std::hex << (int*) &emptyImg.state_flag << std::endl;
    std::cout << "align \t\t: 0x" << std::hex << (int*) &emptyImg.index << std::endl;
    std::cout << "data \t\t: 0x" << std::hex << &emptyImg.data << std::endl;
    std::cout << "===" << std::endl;

        // L3 Cache size      = 12'582'912
        // L1 Cache line size = 64
    const size_t cacheLineSize = getL1CacheLineSize();
    const uint8_t ringCapacity = 1;
    const size_t imageStructureSize = sizeof(imaging::Image) - sizeof(imaging::Image::data) + emptyImg.width * emptyImg.height * emptyImg.channel_size;  // 4'320'016
    const size_t imageMemorySpace = ((imageStructureSize + cacheLineSize - 1) / cacheLineSize) * cacheLineSize;     // 4'320'064

    const uint32_t mmfSize = imageMemorySpace * ringCapacity;           // 17'280'256

    uint8_t* outputMMFMemoryPtr;
    HANDLE outputMMFMap;

    createMMF(mmfNamespace, mmfSize, (void**)&outputMMFMemoryPtr, &outputMMFMap);

    imaging::Image** imageArray = bindMMFSpace(outputMMFMemoryPtr, ringCapacity, imageMemorySpace);

    // producer stream initialisation
    for (uint8_t index = 0; index < ringCapacity; index++) {
        imageArray[index]->width = emptyImg.width;
        imageArray[index]->height = emptyImg.height;
        imageArray[index]->channel_type = emptyImg.channel_type;
        imageArray[index]->channel_size = emptyImg.channel_size;
        imageArray[index]->state_flag.store(0, std::memory_order_release);
        imageArray[index]->index = 45;
        imageArray[index]->data = NULL;
    }

    std::cout << std::dec;

    auto timeout = std::chrono::milliseconds(10);

    // producer stream
    uint8_t imageIndex = 0;
    uint8_t ringIndex = 0;
    while (true) {
        imaging::Image* img = imageArray[ringIndex];

        if (!SpinWaitLock(&(img->state_flag), 100000, std::chrono::nanoseconds(1000000), PROCESS_CONSUMMER_RELEASED, PROCESS_PRODUCER_WORKING)) {
            auto memoryState = img->state_flag.exchange(PROCESS_PRODUCER_WORKING);
            if (memoryState == PROCESS_PRODUCER_RELEASED)
                std::cout << "Missed" << std::endl;
            else if (memoryState == PROCESS_CONSUMMER_WORKING)
                std::cout << "Too long to manage, possibility of memory corruption" << std::endl;       // ici, le consommateur doit verifier que sa zone mémoire soit bien cohérente. que la zone mémoire ne soit pas revenu au producteur avant de flip le flag.
            else
                std::cout << "Force MMF cell byte flip from : " << (int)memoryState << std::endl;
        }

        auto now = std::chrono::system_clock::now();

        auto nanosec = std::chrono::duration_cast<std::chrono::nanoseconds>(now.time_since_epoch()).count();

        memcpy(&img->data, &nanosec, sizeof(long long));

        auto memoryState = img->state_flag.exchange(PROCESS_PRODUCER_RELEASED);

        ringIndex++;
        imageIndex++;

        if (ringIndex == ringCapacity)
            ringIndex = 0;
    }

    free(imageArray);
    closeMMF(outputMMFMemoryPtr, outputMMFMap);
}
