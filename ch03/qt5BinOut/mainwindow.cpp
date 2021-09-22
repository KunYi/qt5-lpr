#include <QApplication>
#include <QFileDialog>
#include <QMessageBox>
#include <QPixmap>
#include <QKeyEvent>
#include <QDebug>
#include "mainwindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent),
      saveImage(nullptr),
      fp(nullptr),
      mTh(nullptr),
      mZ(nullptr)
{
    initUI();
}

MainWindow::~MainWindow()
{
    if (saveImage != nullptr)
        delete saveImage;

    if (fp != nullptr)
        delete fp;

    if (mTh != nullptr)
        delete[] mTh;

    if (mZ != nullptr)
        delete[] mZ;
}

void MainWindow::initUI()
{
    this->resize(800, 600);

    // main area for image display
    imageScene = new ImageViewScene(this);
    imageView = new QGraphicsView(imageScene);
    setCentralWidget(imageView);

    // setup status bar
    mainStatusBar = statusBar();
    mainStatusLabel = new QLabel(mainStatusBar);
    mainStatusBar->addPermanentWidget(mainStatusLabel);
    mainStatusLabel->setText("Image Information will be here!");

    createActions();
    // for mouse right
    this->setContextMenuPolicy(Qt::CustomContextMenu);
    connect(this, SIGNAL(customContextMenuRequested(const QPoint &)),
            this, SLOT(showContextMenu(const QPoint &)));
}

void MainWindow::createActions()
{
    ActionOpen = new QAction("Open", this);
    ActionGray = new QAction(tr("Gray"), this);
    ActionAve = new QAction(tr("Ave"), this);
    ActionBinary = new QAction("Binary", this);
    ActionOutline = new QAction(tr("Outline"), this);

    menuBar()->addAction(ActionOpen);
    menuBar()->addAction(ActionGray);
    menuBar()->addAction(ActionAve);
    menuBar()->addAction(ActionBinary);
    menuBar()->addAction(ActionOutline);


    connect(ActionOpen, SIGNAL(triggered(bool)), this, SLOT(openImage()));
    connect(ActionGray, SIGNAL(triggered(bool)), this, SLOT(showGrayGreen()));
    connect(ActionAve, SIGNAL(triggered(bool)), this, SLOT(showAve()));
    connect(ActionBinary, SIGNAL(triggered(bool)), this, SLOT(showBinary()));
    connect(ActionOutline, SIGNAL(triggered(bool)), this, SLOT(showOutline()));

}

void MainWindow::openImage()
{
    QFileDialog dialog(this);
    dialog.setWindowTitle("Open Image");
    dialog.setFileMode(QFileDialog::ExistingFile);
    dialog.setNameFilter(tr("Images (*.png *.bmp *.jpg)"));
    QStringList filePaths;
    if (dialog.exec()) {
        filePaths = dialog.selectedFiles();

        showImage(filePaths.at(0));
    }
}

void MainWindow::saveToFile()
{
    if (saveImage == nullptr)
        return;

    QFileDialog dialog(this, tr("Save Image"), QString(),
                       tr("Images (*.png *.bmp *.jpg)"));
    dialog.setDefaultSuffix(".png");
    dialog.setAcceptMode(QFileDialog::AcceptSave);
    QString fileName;
    if (dialog.exec()) {
        fileName = dialog.selectedFiles().front();
    }

    if (fileName.isEmpty() || fileName.isNull())
        return;

    saveImage->save(fileName);
}

void MainWindow::showImage(QString path)
{
    imageScene->clear();
    imageView->resetMatrix();

    QImage image(path);

    const int w = image.width();
    const int h = image.height();

    QPixmap img(w, h);
    img.convertFromImage(image);
    if (fp != nullptr) delete fp;
    fp = new FastPixel(image);
    currentImage = imageScene->addPixmap(img);
    imageScene->update();
    imageView->setSceneRect(img.rect());
    QString status = QString("%1, %2x%3, %4 Bytes").arg(path).arg(image.width())
        .arg(image.height()).arg(QFile(path).size());
    mainStatusLabel->setText(status);
    currentImagePath = path;
}

void MainWindow::showGrayRed()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->grayRed();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showGrayGreen()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->grayGreen();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showGrayBlue()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->grayBlue();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showGrayRGB()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->grayRGB();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showGrayRGLow()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->grayRGLow();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showBinary()
{
    if (fp == nullptr) return;

    if (mTh == nullptr) return;

    const int w = fp->width();
    const int h = fp->height();
    const uchar* chG = fp->ch_g;
    const int csize = w * h;
    const int kw = (w / gapDim) + ((w % gapDim) != 0);
    const int kh = (h / gapDim) + ((h % gapDim) != 0);
    const int ksize = kw * kh;

    if (mZ != nullptr) delete[] mZ;
    mZ = new uchar[csize];
    memset(mZ, 0, csize);
    for (int y = 0; y < h; y++) {
        const int i = (y / gapDim);
        for(int x = 0; x < w; x++) {
            const int j = (x / gapDim);
            if (chG[y * w + x] < mTh[i * kw + j])
                mZ[y * w + x] = 1;
        }
    }

    QPixmap *image = new QPixmap();
    QImage *img = fp->BWImg(mZ);
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showNegative()
{
    if (fp == nullptr) return;

    QPixmap *image = new QPixmap();
    QImage *img = fp->negative();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showAve()
{
    if (fp == nullptr) return;

    const int w = fp->width();
    const int h = fp->height();
    const uchar* chG = fp->ch_g;
    const int csize = w * h;
    const int kw = (w / gapDim) + ((w % gapDim) != 0);
    const int kh = (h / gapDim) + ((h % gapDim) != 0);
    const int ksize = kw * kh;

    if (mTh != nullptr) delete[] mTh;
    mTh = new uint[ksize];
    memset(mTh, 0, ksize * sizeof(uint));
    for (int i = 0; i < h; i++) {
        const int y = (i / gapDim);
        for (int j = 0; j < w; j++) {
            const int x = (j / gapDim);
            mTh[y * kw + x] += chG[i * w + j];
        }
    }

    for (int i = 0; i < kh; i++) {
        for (int j = 0; j < kw; j++) {
            mTh[i * kw + j] /= (gapDim * gapDim);
        }
    }

    uchar *Ave = new uchar[csize];
    for (int y = 0; y < h; y++) {
        const int i = (y / gapDim);
        for(int x = 0; x < w; x++) {
            const int j = (x / gapDim);
            Ave[y * w + x] = mTh[i * kw + j];
        }
    }

    QPixmap *image = new QPixmap();
    QImage *img = fp->gray(Ave);
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
    delete[] Ave;
}

QImage* MainWindow::Outline(const uchar* b)
{
    const int w = fp->width();
    const int h = fp->height();
    const int csize = w * h;

    if (mO != nullptr) delete[] mO;
    mO = new uchar[csize];
    memset(mO, 0, csize);
    for (int y = 1; y < (h - 1); y++) {
        for (int x = 1; x < (w - 1); x++) {
            if (b[y * w + x] == 0) continue;
            if (b[y * w + x - w] == 0) { mO[y * w + x] = 1; continue; }
            if (b[y * w + x - 1] == 0) { mO[y * w + x] = 1; continue; }
            if (b[y * w + x + 1] == 0) { mO[y * w + x] = 1; continue; }
            if (b[y * w + x + w] == 0) { mO[y * w + x] = 1; continue; }
        }
    }

    return fp->BWImg(mO);
}

void MainWindow::showOutline()
{
    if ((fp == nullptr) || (mZ == nullptr))return;

    QImage *img =Outline(mZ);
    QPixmap *image = new QPixmap();
    image->convertFromImage(*img);
    imageScene->addPixmap(*image);
    imageScene->update();
    imageView->setSceneRect(image->rect());
    keepImage(img);
    delete image;
}

void MainWindow::showContextMenu(const QPoint &pos)
{
   if (fp == nullptr) return;

   QMenu contextMenu(tr("Context menu"), this);

   QAction action(tr("Save as file"), this);
   connect(&action, SIGNAL(triggered(bool)), this, SLOT(saveToFile()));
   contextMenu.addAction(&action);
   contextMenu.exec(mapToGlobal(pos));
}

void MainWindow::keepImage(QImage *img)
{
    if (saveImage != nullptr)
        delete saveImage;

    saveImage = img;
}
