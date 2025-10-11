#include "HalconCpp.h"
#include <iostream>
#include <vector>
#include <cstring> // pour memset
#include <windows.h>
#include <chrono>

// SEE Mutex interprocesses & multilang (C#, C/C++)

// SEE YML documents
// must present thoses data :
// + ring capacity (each one used bay a thread)
//    [1;x]
//    define maximal ring capacity
// do not integrate those data :
// - MMF_ID

// SEE MESI minimal
// align each images on the size of the L3 cache >> will reduce the MESI
//   > introduce paddings to separate each image in the mmf

// SEE Minimize system calls for data
// SpinWait instead of EventWaitHandle
// 
// Hybrid approach (recommandé pour inter-process)
// SpinWait court → EventWaitHandle :
// Spin quelques centaines de cycles → si le producteur n’a pas fini, alors appel kernel pour dormir → économise CPU.
// Réduit la latence, mais pas totalement zero syscall.
// Très pratique pour flux d’images en temps réel.

using namespace HalconCpp;

void halconProc() {
    try
    {
        // Créer un objet image HALCON
        HObject ho_Image;
        HTuple width, height;

        // Lire une image d'exemple fournie par HALCON
        ReadImage(&ho_Image, "C:\\Users\\quentin.de-potter\\Pictures\\para.jpg");

        // Obtenir ses dimensions
        GetImageSize(ho_Image, &width, &height);
        std::cout << "Dimensions de l'image : "
            << width[0].L() << " x " << height[0].L() << std::endl;

        // Libérer l'image
        ClearObj(ho_Image);
    }
    catch (HalconCpp::HException& e)
    {
        std::cerr << "Erreur HALCON : " << e.ErrorMessage() << std::endl;
        return;
    }

    std::cout << "Programme terminé avec succès." << std::endl;
}

enum ChannelType : uint8_t {
    /// <summary>
    /// Gray scale on one byte.
    /// </summary>
    Y8,
    /// <summary>
    /// RGB organized that way on one byte each.
    /// </summary>
    RGB8,
    /// <summary>
    /// BGR organized that way on one byte each.
    /// </summary>
    BGR8,
    /// <summary>
    /// An empty byte followed by RGB organized that way on one byte each.
    /// </summary>
    XRGB8,
    /// <summary>
    /// RGB followed by an empty byte organized that way on one byte each.
    /// </summary>
    RGBX8,
    /// <summary>
    /// RGB followed by an absorbtion byte organized that way on one byte each.
    /// </summary>
    RGBA8,
    /// <summary>
    /// An empty byte followed by BGR organized that way on one byte each.
    /// </summary>
    XBGR8,
    /// <summary>
    /// BGR followed by an empty byte organized that way on one byte each.
    /// </summary>
    BGRX8,
    /// <summary>
    /// BGR followed by an absorbtion byte organized that way on one byte each.
    /// </summary>
    BGRA8,
    PLANAR = 0x80
};

struct Matrix2 {
    uint16_t width;
    uint16_t height;
    enum ChannelType channel_type;
    uint8_t channel_size;
    uint8_t _cache_alignment_1; // Align data
    uint8_t _cache_alignment_2; // Align data
    uint8_t* data;
};

uint8_t ComputeChannelByteSize(enum ChannelType channelType) {

    switch (channelType) {
    case ChannelType::Y8:
        return 1;
    case ChannelType::BGR8:
    case ChannelType::RGB8:
        return 3;
    case ChannelType::XRGB8:
    case ChannelType::RGBX8:
    case ChannelType::RGBA8:
    case ChannelType::XBGR8:
    case ChannelType::BGRX8:
    case ChannelType::BGRA8:
        return 4;
    default:
        throw new std::runtime_error("Unknown channel type");
    }
}

Matrix2 ConstructMatrix2() {
    Matrix2 mat;
    mat.width = 17;
    mat.height = 18;
    mat.channel_type = ChannelType::BGR8;
    mat.channel_size = ComputeChannelByteSize(mat.channel_type);
    mat.data = (uint8_t*)malloc(mat.width * mat.height * mat.channel_size);
    return mat;
}

int CreateMMF(LPCWSTR MMFID, size_t memorySize, void** outputMemoryPtr, HANDLE* outputHandleMap) {
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

void CloseMMF(void* memoryPtr, HANDLE memoryHandle) {
    UnmapViewOfFile(memoryPtr);
    CloseHandle(memoryHandle);
}


int main()
{
    uint8_t ringCapacity = 1;

    LPCWSTR MMFID = L"Bonjour";
    HANDLE hMap;
    void* pMem;
    HImage img;
    HTuple tupleWidth, tupleHeight, type, numChannels;
    uint16_t width, height;
    uint8_t channelSize;
    Matrix2* matrix;
    size_t imageDataSize, planAllocation, matrixSize;
    void** ringMemoryCells;

    ringMemoryCells = (void**)malloc(ringCapacity);

    ReadImage(&img, "C:\\Users\\quentin.de-potter\\Pictures\\para.jpg");
    GetImageSize(img, &tupleWidth, &tupleHeight);
    GetImageType(img, &type);
    CountChannels(img, &numChannels);
    
    width = tupleWidth[0].L();
    height = tupleHeight[0].L();
    channelSize = numChannels[0].L();

    planAllocation = width * height;
    imageDataSize = planAllocation * channelSize;
    matrixSize = imageDataSize + sizeof(Matrix2) - 1;

    CreateMMF(MMFID, matrixSize * ringCapacity, &pMem, &hMap);
    for (int i = 0; i < ringCapacity; i++) {
        ringMemoryCells[i] = (void*)((size_t)pMem + (matrixSize * i));
        matrix = reinterpret_cast<Matrix2*>(ringMemoryCells[i]);

        std::cout << "width : 0x" << std::hex << &matrix->width << std::endl;
        std::cout << "height : 0x" << std::hex << &matrix->height << std::endl;
        std::cout << "channel_type : 0x" << std::hex << &matrix->channel_type << std::endl;
        std::cout << "channel_size : 0x" << std::hex << &matrix->channel_size << std::endl;
        std::cout << "data : 0x" << std::hex << &matrix->data << std::endl;
        std::cout << "===" << std::endl;

        matrix->width = width;
        matrix->height = height;
        switch (channelSize) {
        case 1:
            matrix->channel_type = ChannelType::Y8;
            break;
        case 3:
            matrix->channel_type = (ChannelType)((uint8_t)ChannelType::PLANAR | (uint8_t)ChannelType::RGB8);
            break;
        case 4:
            matrix->channel_type = (ChannelType)((uint8_t)ChannelType::PLANAR | (uint8_t)ChannelType::RGBX8);
            break;
        }
        matrix->channel_size = channelSize;

        std::cout << "width : 0x" << std::hex << &matrix->width << std::endl;
        std::cout << "height : 0x" << std::hex << &matrix->height << std::endl;
        std::cout << "channel_type : 0x" << std::hex << &matrix->channel_type << std::endl;
        std::cout << "channel_size : 0x" << std::hex << &matrix->channel_size << std::endl;
        std::cout << "data : 0x" << std::hex << &matrix->data << std::endl;
        std::cout << "===" << std::endl;
    }
    matrix = reinterpret_cast<Matrix2*>(ringMemoryCells[0]);

    std::chrono::steady_clock::time_point nowTime, precedentTime;
    
    HTuple tr, tg, tb;
    HTuple ty;

    double somme = 0.0;
    long long compteur = 0;
    const int N = 1000; // afficher la moyenne tous les 1000 nombres

    while (true) {
        precedentTime = std::chrono::high_resolution_clock::now();
        switch (channelSize) {
        case 3:
            break;
        case 0:
            GetImagePointer3(img, &tr, &tg, &tb, &type, &tupleWidth, &tupleHeight);

            std::memcpy(&matrix->data, reinterpret_cast<uint8_t*>(tr.L()), planAllocation);
            std::memcpy((char*)&matrix->data + planAllocation, reinterpret_cast<void*>(tg.L()), planAllocation);
            std::memcpy((char*)&matrix->data + 2 * planAllocation, reinterpret_cast<void*>(tb.L()), planAllocation);
            break;
        case 1:
            GetImagePointer1(img, &ty, &type, &tupleWidth, &tupleHeight);

            std::memcpy(matrix->data, reinterpret_cast<void*>(ty.L()), planAllocation);
            break;
        case 4:
        default:
            break;
        }
        nowTime = std::chrono::high_resolution_clock::now();

        auto differenciation = std::chrono::duration_cast<std::chrono::microseconds>(nowTime - precedentTime);

        //std::cout << std::dec << differenciation.count() << "\t micro-secondes" << std::endl;


        somme += differenciation.count();
        compteur++;

        if (compteur % N == 0) {
            double moyenne = somme / compteur;
            std::cout << "Moyenne après " << compteur << " relevés : " << moyenne << " micro-secondes." << std::endl;
        }

        Sleep(2);
    }

    CloseMMF(pMem, hMap);
}
