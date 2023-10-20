#ifndef CARWIDGET_H
#define CARWIDGET_H

#include <QWidget>

namespace Ui {
class CarWidget;
}

class CarWidget : public QWidget
{
    Q_OBJECT
    const QString carName;
    const unsigned short model;
    const unsigned short price;
    const QStringList parts;
public:
    explicit CarWidget(const QString& name, unsigned short model, const QStringList& parts, unsigned short price, QWidget *parent = nullptr);
    ~CarWidget();

    void hideRetailInfo();
    void showRetailInfo();
    unsigned short getPrice() const;

    QString getCarName() const;

    unsigned short getModel() const;

    QStringList getParts() const;

private:
    Ui::CarWidget *ui;
signals:
    void carSelected(CarWidget*);

    // QWidget interface
protected:
    void mousePressEvent(QMouseEvent *event);
};

#endif // CARWIDGET_H
