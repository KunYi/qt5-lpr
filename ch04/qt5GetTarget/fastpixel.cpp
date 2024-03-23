
#include "fastpixel.h"

FastPixel::FastPixel(QImage img) :
    data(img),
    ch_r(nullptr),
    ch_g(nullptr),
    ch_b(nullptr),
    m_Th(nullptr),
    m_width(0),
    m_height(0)
{
    const int w = data.width();
    const int h = data.height();
    m_width = w;
    m_height = h;
    const int size = w * h;
    ch_r = new uchar[size];
    ch_g = new uchar[size];
    ch_b = new uchar[size];

    QRgb *ptr = (QRgb *)data.bits();
    for (int y = 0; y < h; y++) {
       int idx = (y*w);
       QRgb *src = ptr + idx;
       uchar *dstR = ch_r + idx;
       uchar *dstG = ch_g + idx;
       uchar *dstB = ch_b + idx;
       for (int x = 0; x < w; x++) {
           dstR[x] = qRed(src[x]);
           dstG[x] = qGreen(src[x]);
           dstB[x] = qBlue(src[x]);
       }
    }

}

FastPixel::~FastPixel()
{
    if (ch_r != nullptr)
        delete[] ch_r;
    if (ch_g != nullptr)
        delete[] ch_g;
    if (ch_b != nullptr)
        delete[] ch_b;
    if (m_Th != nullptr)
        delete[] m_Th;
}

QImage * FastPixel::gray(const uchar* tmp)
{
    const int w = data.width();
    const int h = data.height();
    QImage *img = new QImage(w, h, data.format());

    QRgb *ptr = (QRgb *)img->bits();
    for (int y = 0; y < h; y++) {
       int idx = (y*w);
       const uchar *src = tmp + idx;
       QRgb *dst = ptr + idx;
       for (int x = 0; x < w; x++) {
           dst[x] = qRgb(src[x], src[x], src[x]);
       }
    }

    return img;
}

QImage *FastPixel::grayRed()
{
    return gray(ch_r);
}

QImage *FastPixel::grayGreen()
{
    return gray(ch_g);
}

QImage *FastPixel::grayBlue()
{
    return gray(ch_b);
}

QImage *FastPixel::grayRGB()
{
    const int w = data.width();
    const int h = data.height();
    uchar *tmp = new uchar[(w * h)];

    for (int y = 0; y < h; ++y) {
       int idx = (y*w);
       uchar *srcR = ch_r + idx;
       uchar *srcG = ch_g + idx;
       uchar *srcB = ch_b + idx;
       uchar *dst = tmp + idx;
       for (int x = 0; x < w; ++x) {
           int color = ((int)srcR[x] * 0.299f + (int)srcG[x] * 0.587f + (int)srcB[x] * 0.114f);
           dst[x] = ((color > 255) ? 255 : color) & 0xff;
       }
    }

    QImage *img = gray(tmp);
    delete[] tmp;
    return img;
}

QImage *FastPixel::grayRGLow()
{
    const int w = data.width();
    const int h = data.height();
    uchar *tmp = new uchar[(w * h)];

    for (int y = 0; y < h; ++y) {
       int idx = (y*w);
       uchar *srcR = ch_r + idx;
       uchar *srcG = ch_g + idx;
       uchar *dst = tmp + idx;
       for (int x = 0; x < w; ++x) {
           dst[x] = (srcR[x] > srcG[x]) ? srcG[x] : srcR[x];
       }
    }

    QImage *img = gray(tmp);
    delete[] tmp;
    return img;
}

QImage *FastPixel::BWImg(const uchar* tmp)
{
    const int w = data.width();
    const int h = data.height();
    QImage *img = new QImage(w, h, data.format());

    QRgb *ptr = (QRgb *)img->bits();
    for (int y = 0; y < h; y++) {
       int idx = (y*w);
       const uchar *src = tmp + idx;
       QRgb *dst = ptr + idx;
       for (int x = 0; x < w; x++) {
           dst[x] = (src[x] == 1) ? qRgb(0, 0, 0) : qRgb(255, 255, 255);
       }
    }

    return img;
}

void FastPixel::calcThreshold(int gapDim)
{
    const int w = data.width();
    const int h = data.height();
    const uchar* chG = this->ch_g;
    const int csize = w * h;
    const int kw = (w / gapDim) + ((w % gapDim) != 0);
    const int kh = (h / gapDim) + ((h % gapDim) != 0);
    const int ksize = kw * kh;

    if (m_Th != nullptr) delete[] m_Th;
    m_Th = new uint[ksize];
    memset(m_Th, 0, ksize * sizeof(uint));
    for (int i = 0; i < h; i++) {
        const int y = (i / gapDim);
        for (int j = 0; j < w; j++) {
            const int x = (j / gapDim);
            m_Th[y * kw + x] += chG[i * w + j];
        }
    }

    for (int i = 0; i < kh; i++) {
        for (int j = 0; j < kw; j++) {
            m_Th[i * kw + j] /= (gapDim * gapDim);
        }
    }
}

QImage *FastPixel::convBinary(int gapDim)
{
    if (m_Th == nullptr)
      return (QImage *)nullptr;

    const int w = data.width();
    const int h = data.height();
    const uchar* chG = this->ch_g;
    const int kw = (w / gapDim) + ((w % gapDim) != 0);
    const int kh = (h / gapDim) + ((h % gapDim) != 0);

    return (QImage *)nullptr;
}

QImage *FastPixel::binary()
{
    const int w = data.width();
    const int h = data.height();
    uchar *tmp = new uchar[(w * h)];
    memset(tmp, 0,  (w * h));
    for (int y = 0; y < h; ++y) {
       int idx = (y*w);
       uchar *src = ch_g + idx;
       uchar *dst = tmp + idx;
       for (int x = 0; x < w; ++x) {
           if (src[x] < 128)
               dst[x] = 1;
       }
    }

    QImage *img = BWImg(tmp);
    delete[] tmp;
    return img;
}

QImage *FastPixel::negative()
{
    const int w = data.width();
    const int h = data.height();
    uchar *tmp = new uchar[(w * h)];
    memset(tmp, 0,  (w * h));
    for (int y = 0; y < h; ++y) {
       int idx = (y*w);
       uchar *src = ch_g + idx;
       uchar *dst = tmp + idx;
       for (int x = 0; x < w; ++x) {
          dst[x] = 255 - src[x];
       }
    }

    QImage *img = gray(tmp);
    delete[] tmp;
    return img;
}
