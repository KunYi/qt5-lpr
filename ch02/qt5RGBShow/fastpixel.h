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
    QImage *gray(const uchar* tmp);
    QImage *BWImg(const uchar* tmp);

    QImage data;
    uchar *ch_r;
    uchar *ch_g;
    uchar *ch_b;
};

#endif // _FASTPIXEL_H
