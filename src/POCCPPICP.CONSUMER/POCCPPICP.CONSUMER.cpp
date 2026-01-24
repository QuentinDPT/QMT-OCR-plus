#include <iostream>
#include <windows.h>

#include "Image.h"
#include <vector>
#include <thread>
#include <chrono>


#define PROCESS_CONSUMMER_RELEASED  0x10
#define PROCESS_CONSUMMER_WORKING   0x11
#define PROCESS_PRODUCER_RELEASED   0x20
#define PROCESS_PRODUCER_WORKING    0x21


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


void threadedRead(imaging::Image* img) {
    while (true) {
        while (!SpinWaitLock(&(img->state_flag), 100000, std::chrono::nanoseconds(0), PROCESS_PRODUCER_RELEASED, PROCESS_CONSUMMER_WORKING));

        long long timestamp = (long long)img->data;

        auto validationState = img->state_flag.exchange(PROCESS_CONSUMMER_RELEASED);

        if (validationState != PROCESS_CONSUMMER_WORKING) {
            std::cout << "Communication invalidation!\n";
        }
    }
}

int main()
{
    //std::string waiit;
    //std::cin >> waiit;

    imaging::Image emptyImg;
    emptyImg.channel_type = imaging::ImagePixelFormat::RGB8;
    emptyImg.channel_size = 3;
    emptyImg.width = 1600;
    emptyImg.height = 900;
    emptyImg.data = nullptr;
    const size_t cacheLineSize = 64;
    const size_t imageStructureSize = sizeof(imaging::Image) - sizeof(imaging::Image::data) + emptyImg.width * emptyImg.height * emptyImg.channel_size;
    const size_t imageMemorySpace = ((imageStructureSize + cacheLineSize - 1) / cacheLineSize) * cacheLineSize;

    std::cout << "Hello World!\n";

    const wchar_t* mmfName = L"YEP";  // nom défini par le créateur

    HANDLE hMapFile = OpenFileMappingW(
        FILE_MAP_ALL_ACCESS, // ou FILE_MAP_READ si lecture seule
        FALSE,
        mmfName
    );

    if (hMapFile == NULL) {
        std::cerr << "x OpenFileMapping failed (" << GetLastError() << ")" << std::endl;
        return 1;
    }

    void* pBuf = MapViewOfFile(
        hMapFile,
        FILE_MAP_ALL_ACCESS,
        0, 0,
        0 // 0 = toute la taille
    );

    if (pBuf == NULL) {
        std::cerr << "x MapViewOfFile failed (" << GetLastError() << ")" << std::endl;
        CloseHandle(hMapFile);
        return 1;
    }

    threadedRead((imaging::Image*)pBuf);

    
    std::vector<std::thread> threads;

    for (int i = 0; i < 1; ++i) {
        //threads.emplace_back(threadedRead, (imaging::Image*)pBuf + imageMemorySpace * i);
    }

    while(true){}


    UnmapViewOfFile(pBuf);
    CloseHandle(hMapFile);

    return 0;
}