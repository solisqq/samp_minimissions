#include "carwidget.h"
#include "ui_carwidget.h"

#include <QMessageBox>
#include <QPushButton>

unsigned short CarWidget::getPrice() const
{
    return price;
}

QString CarWidget::getCarName() const
{
    return carName;
}

unsigned short CarWidget::getModel() const
{
    return model;
}

QStringList CarWidget::getParts() const
{
    return parts;
}

CarWidget::CarWidget(const QString& name, unsigned short model, const QStringList& parts, unsigned short price, QWidget *parent) :
    QWidget(parent),
    carName(name),
    model(model),
    price(price),
    parts(parts),
    ui(new Ui::CarWidget)
{
    ui->setupUi(this);
    ui->carMainWidget->setStyleSheet("#carMainWidget {border-image:url(:/cars/cars/"+name+".jpg);}");
    ui->nameLabel->setText(name);
    ui->priceLabel->setText(QString::number(price));
    ui->tuningLabel->setText(QString::number(parts.length()));
}

void CarWidget::hideRetailInfo() {
    ui->widget_5->hide();
}

void CarWidget::showRetailInfo() {
    ui->widget_5->show();
}

void CarWidget::mousePressEvent(QMouseEvent *event)
{
    emit carSelected(this);
}

CarWidget::~CarWidget()
{
    delete ui;
}
