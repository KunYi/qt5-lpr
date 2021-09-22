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
    // for mouse right click contextMenu
    void showContextMenu(const QPoint &);
    void saveToFile(void);

private:
    QAction *ActionOpen;
    QAction *ActionRed;
    QAction *ActionGreen;
    QAction *ActionBlue;
    QAction *ActionRGB;
    QAction *ActionRGLow;
    QAction *ActionBinary;
    QAction *ActionNegative;

    QStatusBar *mainStatusBar;
    QLabel *mainStatusLabel;

    QGraphicsScene *imageScene;
    QGraphicsView *imageView;

    QImage *saveImage;
    QString currentImagePath;
    QGraphicsPixmapItem *currentImage;
    FastPixel *fp;

private:
    void keepImage(QImage *img);
};
#endif // MAINWINDOW_H
