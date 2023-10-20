#ifndef CARSDIALOG_H
#define CARSDIALOG_H

#include <QDialog>
#include <QFile>
#include <QFrame>
#include <CarWidget.h>

namespace Ui {
class CarsDialog;
}

class CarsDialog : public QDialog
{
    Q_OBJECT
    static QFile carsInfo;
public:
    explicit CarsDialog(QWidget *parent = nullptr);
    ~CarsDialog();

private slots:
    void handleCarSelection(CarWidget *carWidget);
signals:
    void requestBuy(const QString& vehName, unsigned int model, unsigned int price);
private:
    Ui::CarsDialog *ui;
};

#endif // CARSDIALOG_H
