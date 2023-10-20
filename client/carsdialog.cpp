#include "carsdialog.h"
#include "ui_carsdialog.h"
#include "static/StylesheetLoader.h"
#include <QMessageBox>
#include <QPushButton>

QFile CarsDialog::carsInfo = QFile(":/data/cars.csv");

CarsDialog::CarsDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::CarsDialog)
{
    ui->setupUi(this);
    StylesheetLoader::loadInto("main.css", this);
    carsInfo.open(QFile::ReadOnly);
    if(carsInfo.isOpen()) {
        auto carsCSV = QString::fromUtf8(carsInfo.readAll());
        auto lines = carsCSV.split('\n');
        int i=0;
        QWidget* container{};
        for(const auto& line : lines) {
            auto carInfo = line.split(';');
            if(carInfo.count()!=4) continue;
            auto carwdg = new CarWidget(carInfo[0], carInfo[1].toUShort(), carInfo[2].split('&'), carInfo[3].toUShort());
            connect(carwdg, &CarWidget::carSelected, this, &CarsDialog::handleCarSelection);
            if(i%4==0) {
                container = new QWidget();
                container->setLayout(new QHBoxLayout());
                container->layout()->setContentsMargins(2,2,2,2);
                container->layout()->setSpacing(4);
                ui->carDialogWidget->layout()->addWidget(container);
            }
            container->layout()->addWidget(carwdg);
            i++;
        }
    }
}

void CarsDialog::handleCarSelection(CarWidget* carWidget) {
    QMessageBox* msgBox = new QMessageBox(this);
    msgBox->setWindowFlag(Qt::FramelessWindowHint,true);
    msgBox->setWindowOpacity(0.96);
    msgBox->setText("Zamierzasz kupić pojazd za "+QString::number(carWidget->getPrice())+".");
    msgBox->setInformativeText("Czy jesteś pewien?");
    QPushButton* buyBtn = new QPushButton("Tak");
    msgBox->addButton(buyBtn, QMessageBox::AcceptRole);
    msgBox->addButton(new QPushButton("Anuluj"), QMessageBox::RejectRole);
    msgBox->open();
    msgBox->setAttribute(Qt::WA_DeleteOnClose);
    connect(buyBtn, &QPushButton::clicked, this, [this, carWidget]() {
        emit requestBuy(carWidget->getCarName(), carWidget->getModel(), carWidget->getPrice());
    });
}

CarsDialog::~CarsDialog()
{
    delete ui;
}
