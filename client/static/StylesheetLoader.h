#ifndef STYLESHEETLOADER_H
#define STYLESHEETLOADER_H

#include <QString>
#include <QWidget>

class StylesheetLoader {
public:
    static QString path;
    static void loadInto(const QString& fileName, QWidget* widget);
};

#endif // STYLESHEETLOADER_H
