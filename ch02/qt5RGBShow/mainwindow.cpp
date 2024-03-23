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
      fp(nullptr)
{
    initUI();
}

MainWindow::~MainWindow()
{
    if (saveImage != nullptr)
        delete saveImage;

    if (fp != nullptr)
        delete fp;
}

void MainWindow::initUI()
{
    this->resize(800, 600);

    // main area for image display
    imageScene = new QGraphicsScene(this);
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
    ActionRed = new QAction("Red", this);
    ActionGreen = new QAction("Green", this);
    ActionBlue = new QAction("Blue", this);
    ActionRGB = new QAction("RGB", this);
    ActionRGLow = new QAction("RG Low", this);
    ActionBinary = new QAction("Binary", this);
    ActionNegative = new QAction("Negative", this);

    menuBar()->addAction(ActionOpen);
    menuBar()->addAction(ActionRed);
    menuBar()->addAction(ActionGreen);
    menuBar()->addAction(ActionBlue);
    menuBar()->addAction(ActionRGB);
    menuBar()->addAction(ActionRGLow);
    menuBar()->addAction(ActionBinary);
    menuBar()->addAction(ActionNegative);

    connect(ActionOpen, SIGNAL(triggered(bool)), this, SLOT(openImage()));
    connect(ActionRed, SIGNAL(triggered(bool)), this, SLOT(showGrayRed()));
    connect(ActionGreen, SIGNAL(triggered(bool)), this, SLOT(showGrayGreen()));
    connect(ActionBlue, SIGNAL(triggered(bool)), this, SLOT(showGrayBlue()));
    connect(ActionRGB, SIGNAL(triggered(bool)), this, SLOT(showGrayRGB()));
    connect(ActionRGLow, SIGNAL(triggered(bool)), this, SLOT(showGrayRGLow()));
    connect(ActionBinary, SIGNAL(triggered(bool)), this, SLOT(showBinary()));
    connect(ActionNegative, SIGNAL(triggered(bool)), this, SLOT(showNegative()));
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
    imageView->resetTransform();
    QImage image(path);
    QPixmap img(image.width(), image.height());
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

    QPixmap *image = new QPixmap();
    QImage *img = fp->binary();
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
