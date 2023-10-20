#include "StylesheetLoader.h"
#include <QFile>
#include <QDebug>
#include <QString>

QString StylesheetLoader::path = ":/css/static/";
void StylesheetLoader::loadInto(const QString& fileName, QWidget* widget) {
    QFile file(path+fileName);
    if(!file.exists()) {qDebug()<<"Failed to load css "+fileName+" ("+file.fileName()+")"; return;}
    if(file.open(QFile::ReadOnly)) {
        if(widget) {
            QString data = file.readAll();
            widget->setStyleSheet(data);
            //qDebug()<<data;
        }
        else "Failed to load css "+fileName+" into QWidget (invalid QWidget)";
    } else "Failed to load css "+fileName;
}
