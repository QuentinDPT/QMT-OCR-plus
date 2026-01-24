#pragma once

#include "ImagePixelFormat.h"
#include <cstdint>

namespace imaging {
    struct Image
    {
        uint16_t width;                     // 0x00 - 2 bytes   ?
                                            // 0x01             ?
        uint16_t height;                    // 0x02 - 2 bytes   ?
                                            // 0x03             ?
        ImagePixelFormat channel_type;      // 0x04 - 1 byte    ?
        uint8_t channel_size;               // 0x05 - 1 byte    ?
        std::atomic<uint8_t> state_flag;    // 0x06 - 1 byte    ?
        uint8_t index;                      // 0x07 - 1 byte    ?
        uint8_t* data;                      // 0x08 - 8 bytes   ?   
                                            // 0x09             ?
                                            // 0x0A             ?
                                            // 0x0B             ?
                                            // 0x0C             ?
                                            // 0x0D             ?
                                            // 0x0E             ?
                                            // 0x0F             ?
    };
}
