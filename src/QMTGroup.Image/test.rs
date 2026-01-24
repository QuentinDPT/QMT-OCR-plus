use std::io;

enum DataType{
    Y_8,
    RGB_8,
    BGR_8,
    XRGB_8,
    RGBX_8,
    RGBA_8,
    XBGR_8,
    BGRX_8,
    BGRA_8,
}

struct Matrix {
    channels: u8,
    channel_type: DataType,
    width:usize,
    height:usize,
    data:Vec<u8>,
}

impl Matrix {
    pub fn new(width: usize, height: usize, channels: u8) -> Self {
        let size = width * height * channels as usize;
        Matrix {
            channels,
            channel_type: DataType::RGB_8,
            width,
            height,
            data:vec![0; size],
        }
    }
    
    pub fn get_pixel(&self) -> vec<u8> {
        
    }
}

fn main() {
    let image = Matrix::new(16,9,3);
    
    println!("yeah!");
}