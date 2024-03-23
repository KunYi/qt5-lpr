#ifndef _FASTPIXEL_H
#define _FASTPIXEL_H

#include <QPixmap>
#include <QBitmap>

struct Point {
    int x, y;
};

struct TgInfo {
    int np; // Number of target points
    std::vector<Point> P; // Vector of 2D target points
    short xmn, xmx, ymn, ymx; // Minimum and maximum coordinates
    int cx, cy; // Center coordinates of the target
    int width, height; // Width and height of the target
    int pm; // Contrast intensity between target and background
    int ID; // Sorting ID based on contrast
};

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

    uint *m_Th; // Threshold
    uchar *m_bin;

public:
    int width(void) { return m_width; }
    int height(void) { return m_height; }
    void calcThreshold(int gapDim);
    QImage *gray(const uchar* tmp);
    QImage *BWImg(const uchar* tmp);
    QImage *convBinary(int gapDim);

public:
    uchar *ch_r;
    uchar *ch_g;
    uchar *ch_b;
};

#endif // _FASTPIXEL_H
