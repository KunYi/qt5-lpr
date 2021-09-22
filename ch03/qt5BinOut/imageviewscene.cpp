#include <QApplication>
#include <QDebug>
#include <QGraphicsSceneMouseEvent>
#include "imageviewscene.h"

void ImageViewScene::mousePressEvent(QGraphicsSceneMouseEvent *event)
{
	if (event->button() == Qt::LeftButton)
    	qDebug() << "mousePressEvent:" << event->button()
	    << ", x:" << event->scenePos().x() << ", y:" << event->scenePos().x();
}
