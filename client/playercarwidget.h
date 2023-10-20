#ifndef PLAYERCARWIDGET_H
#define PLAYERCARWIDGET_H

#include "connection.h"

#include <QWidget>
#include <CarWidget.h>
#include <CarsDialog.h>

namespace Ui {
class PlayerCarWidget;
}

class PlayerCarWidget : public QWidget
{
    Q_OBJECT
    static int vehicleColors[256];
    TCPMsg carInfoMsg;
    TCPMsg buyCarMsg;
    CarsDialog* carsDialog{};
    QString login{};
public:
    explicit PlayerCarWidget(QWidget *parent = nullptr);
    ~PlayerCarWidget();
signals:
    void refreshUserData(const QString& login);
public slots:
    void handleBuy(const QString &name, unsigned int model, unsigned int price);
    void handleNoCar();
    void logedIn(const QString &login);
    void handleCar(const QString &name, unsigned int model, const QStringList &partsOwned = QStringList(), unsigned short colorId = 75);
private:
    Ui::PlayerCarWidget *ui;
};

#endif // PLAYERCARWIDGET_H
