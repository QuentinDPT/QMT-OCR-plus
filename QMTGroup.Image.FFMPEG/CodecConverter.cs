using FFmpeg.AutoGen;
using QMTGroup.Image.Interface;
using System.Runtime.InteropServices;

namespace QMTGroup.Image.FFMPEG
{
    public class CodecConverter : IJpegConverter
    {
        public CodecConverter()
        { }

        public byte[] ConvertToJpeg(Matrix image, int quality)
        {
            if (image.Channels == 1)
                return ConvertRawToJpeg(image.Data, (int)image.Width, (int)image.Height, quality, AVPixelFormat.AV_PIX_FMT_GRAY8);

            if (image.Channels == 3)
                return ConvertRawToJpeg(image.Data, (int)image.Width, (int)image.Height, quality, AVPixelFormat.AV_PIX_FMT_RGB24);

            if (image.Channels == 4)
                return ConvertRawToJpeg(image.Data, (int)image.Width, (int)image.Height, quality, AVPixelFormat.AV_PIX_FMT_RGBA);

            return ConvertRawToJpeg(image.Data, (int)image.Width, (int)image.Height, quality, AVPixelFormat.AV_PIX_FMT_GRAY8);
        }

        public byte[] ConvertToJpeg(Matrix image) => ConvertToJpeg(image, 1);

        private byte[] ConvertRawToJpeg(Span<byte> raw, int width, int height, int quality, AVPixelFormat rawStorageFormat)
        {
            unsafe
            {
                // Trouver le codec JPEG
                AVCodec* codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
                if (codec == null)
                {
                    throw new InvalidOperationException("Codec JPEG not found.");
                }

                // Créer un contexte pour le codec
                AVCodecContext* codecContext = ffmpeg.avcodec_alloc_context3(codec);
                if (codecContext == null)
                {
                    throw new InvalidOperationException("Impossible to create codec context.");
                }

                // Paramètres du codec JPEG
                codecContext->width = width;
                codecContext->height = height;
                codecContext->pix_fmt = rawStorageFormat;
                codecContext->bit_rate = 400000; // Taux de bits pour la compression
                codecContext->compression_level = quality;
                codecContext->time_base = new AVRational { num = 1, den = 25 }; // Frame rate
                ffmpeg.avcodec_open2(codecContext, codec, null);

                // Créer un AVFrame pour stocker les données de l'image
                AVFrame* frame = ffmpeg.av_frame_alloc();
                if (frame == null)
                {
                    throw new InvalidOperationException("Unable to allocate AVFrame.");
                }

                // Allouer de la mémoire pour le frame
                ffmpeg.av_frame_get_buffer(frame, 32); // 32 est une taille d'alignement

                // Remplir les données de l'AVFrame à partir du tableau raw
                byte_ptrArray4 data;
                int_array4 linesize;
                fixed (byte* pRaw = raw)
                {
                    ffmpeg.av_image_fill_arrays(ref data, ref linesize, pRaw, rawStorageFormat, width, height, 1);
                }
                frame->data.UpdateFrom(data);
                frame->linesize.UpdateFrom(linesize);

                // Créer un paquet pour stocker les données encodées
                AVPacket* pkt = ffmpeg.av_packet_alloc();
                if (pkt == null)
                {
                    throw new InvalidOperationException("Impossible d'allouer le paquet AVPacket.");
                }

                // Envoyer le frame au codec pour l'encodage
                ffmpeg.avcodec_send_frame(codecContext, frame);

                // Recevoir le paquet encodé
                ffmpeg.avcodec_receive_packet(codecContext, pkt);

                // Copier les données JPEG dans un tableau de bytes
                byte[] jpegData = new byte[pkt->size];
                Marshal.Copy((IntPtr)pkt->data, jpegData, 0, pkt->size);

                // Libérer les ressources
                ffmpeg.av_packet_free(&pkt);
                ffmpeg.av_frame_free(&frame);
                ffmpeg.avcodec_free_context(&codecContext);

                return jpegData;
            }
        }
    }
}
