#ifndef _FASTPIXEL_H
#define _FASTPIXEL_H

#include <QPixmap>
#include <QBitmap>

class  FastPixel {

public:
    FastPixel(QImage img);
    ~FastPixel();

    QImage *grayRed(void);
    QImage *grayGreen(void);
    QImage *grayBlue(void);
    QImage *grayRGB(void);
    QImage *grayRGLow(void);
    QImage *binary(void);
    QImage *negative(void);

private:
    QImage data;

    int m_width;
    int m_height;

public:
    int width(void) { return m_width; }
    int height(void) { return m_height; }
    QImage *gray(const uchar* tmp);
    QImage *BWImg(const uchar* tmp);

public:
    uchar *ch_r;
    uchar *ch_g;
    uchar *ch_b;
};

#endif // _FASTPIXEL_H
