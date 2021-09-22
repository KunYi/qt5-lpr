
#ifndef _imageview_h_
#define _imageview_h_

#include <QGraphicsScene>

class ImageViewScene : public QGraphicsScene
{
	Q_OBJECT
public:
	ImageViewScene(QObject *parent = nullptr) : QGraphicsScene(parent) {};

protected:
    void mousePressEvent(QGraphicsSceneMouseEvent *mouseEvent) override;
};

#endif
