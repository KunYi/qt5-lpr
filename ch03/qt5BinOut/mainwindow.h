#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QMenuBar>
#include <QToolBar>
#include <QAction>
#include <QGraphicsScene>
#include <QGraphicsView>
#include <QStatusBar>
#include <QLabel>
#include <QGraphicsPixmapItem>
#include "fastpixel.h"
#include "imageviewscene.h"

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private:
    void initUI();
    void createActions();
    void showImage(QString);

private slots:
    void openImage();
    void showGrayRed(void);
    void showGrayGreen(void);
    void showGrayBlue(void);
    void showGrayRGB(void);
    void showGrayRGLow(void);
    void showBinary(void);
    void showNegative(void);
    void showAve(void);
    void showOutline(void);

    // for mouse right click contextMenu
    void showContextMenu(const QPoint &);
    void saveToFile(void);

private:
    QAction *ActionOpen;
    QAction *ActionBinary;
    QAction *ActionGray;
    QAction *ActionAve;
    QAction *ActionOutline;

    QStatusBar *mainStatusBar;
    QLabel *mainStatusLabel;

    ImageViewScene *imageScene;
    QGraphicsView *imageView;

    QImage *saveImage;
    QString currentImagePath;
    QGraphicsPixmapItem *currentImage;
    FastPixel *fp;

    const int gapDim = 40;
    uint *mTh;
    uchar *mZ; // for Binary
    uchar *mO; // for Outline

private:
    void keepImage(QImage *img);
    QImage* Outline(const uchar* b);

    friend class ImageViewScene;
};
#endif // MAINWINDOW_H
