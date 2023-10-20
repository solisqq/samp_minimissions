#include "playercarwidget.h"
#include "ui_playercarwidget.h"

#include <QMessageBox>

int PlayerCarWidget::vehicleColors[256] = {
    // The existing colours from San Andreas
    0x000000, 0xF5F5F5, 0x2A77A1, 0x840410, 0x263739, 0x86446E, 0xD78E10, 0x4C75B7, 0xBDBEC6, 0x5E7072,
    0x46597A, 0x656A79, 0x5D7E8D, 0x58595A, 0xD6DAD6, 0x9CA1A3, 0x335F3F, 0x730E1A, 0x7B0A2A, 0x9F9D94,
    0x3B4E78, 0x732E3E, 0x691E3B, 0x96918C, 0x515459, 0x3F3E45, 0xA5A9A7, 0x635C5A, 0x3D4A68, 0x979592,
    0x421F21, 0x5F272B, 0x8494AB, 0x767B7C, 0x646464, 0x5A5752, 0x252527, 0x2D3A35, 0x93A396, 0x6D7A88,
    0x221918, 0x6F675F, 0x7C1C2A, 0x5F0A15, 0x193826, 0x5D1B20, 0x9D9872, 0x7A7560, 0x989586, 0xADB0B0,
    0x848988, 0x304F45, 0x4D6268, 0x162248, 0x272F4B, 0x7D6256, 0x9EA4AB, 0x9C8D71, 0x6D1822, 0x4E6881,
    0x9C9C98, 0x917347, 0x661C26, 0x949D9F, 0xA4A7A5, 0x8E8C46, 0x341A1E, 0x6A7A8C, 0xAAAD8E, 0xAB988F,
    0x851F2E, 0x6F8297, 0x585853, 0x9AA790, 0x601A23, 0x20202C, 0xA4A096, 0xAA9D84, 0x78222B, 0x0E316D,
    0x722A3F, 0x7B715E, 0x741D28, 0x1E2E32, 0x4D322F, 0x7C1B44, 0x2E5B20, 0x395A83, 0x6D2837, 0xA7A28F,
    0xAFB1B1, 0x364155, 0x6D6C6E, 0x0F6A89, 0x204B6B, 0x2B3E57, 0x9B9F9D, 0x6C8495, 0x4D8495, 0xAE9B7F,
    0x406C8F, 0x1F253B, 0xAB9276, 0x134573, 0x96816C, 0x64686A, 0x105082, 0xA19983, 0x385694, 0x525661,
    0x7F6956, 0x8C929A, 0x596E87, 0x473532, 0x44624F, 0x730A27, 0x223457, 0x640D1B, 0xA3ADC6, 0x695853,
    0x9B8B80, 0x620B1C, 0x5B5D5E, 0x624428, 0x731827, 0x1B376D, 0xEC6AAE, 0x000000, 0x177517, 0x210606,
    0x125478, 0x452A0D, 0x571E1E, 0x010701, 0x25225A, 0x2C89AA, 0x8A4DBD, 0x35963A, 0xB7B7B7, 0x464C8D,
    0x84888C, 0x817867, 0x817A26, 0x6A506F, 0x583E6F, 0x8CB972, 0x824F78, 0x6D276A, 0x1E1D13, 0x1E1306,
    0x1F2518, 0x2C4531, 0x1E4C99, 0x2E5F43, 0x1E9948, 0x1E9999, 0x999976, 0x7C8499, 0x992E1E, 0x2C1E08,
    0x142407, 0x993E4D, 0x1E4C99, 0x198181, 0x1A292A, 0x16616F, 0x1B6687, 0x6C3F99, 0x481A0E, 0x7A7399,
    0x746D99, 0x53387E, 0x222407, 0x3E190C, 0x46210E, 0x991E1E, 0x8D4C8D, 0x805B80, 0x7B3E7E, 0x3C1737,
    0x733517, 0x781818, 0x83341A, 0x8E2F1C, 0x7E3E53, 0x7C6D7C, 0x020C02, 0x072407, 0x163012, 0x16301B,
    0x642B4F, 0x368452, 0x999590, 0x818D96, 0x99991E, 0x7F994C, 0x839292, 0x788222, 0x2B3C99, 0x3A3A0B,
    0x8A794E, 0x0E1F49, 0x15371C, 0x15273A, 0x375775, 0x060820, 0x071326, 0x20394B, 0x2C5089, 0x15426C,
    0x103250, 0x241663, 0x692015, 0x8C8D94, 0x516013, 0x090F02, 0x8C573A, 0x52888E, 0x995C52, 0x99581E,
    0x993A63, 0x998F4E, 0x99311E, 0x0D1842, 0x521E1E, 0x42420D, 0x4C991E, 0x082A1D, 0x96821D, 0x197F19,
    0x3B141F, 0x745217, 0x893F8D, 0x7E1A6C, 0x0B370B, 0x27450D, 0x071F24, 0x784573, 0x8A653A, 0x732617,
    0x319490, 0x56941D, 0x59163D, 0x1B8A2F, 0x38160B, 0x041804, 0x355D8E, 0x2E3F5B, 0x561A28, 0x4E0E27,
    0x706C67, 0x3B3E42, 0x2E2D33, 0x7B7E7D, 0x4A4442, 0x28344E
};

PlayerCarWidget::PlayerCarWidget(QWidget *parent) :
    QWidget(parent),
    carInfoMsg("get_car"),
    buyCarMsg("buy_car"),
    ui(new Ui::PlayerCarWidget)
{
    ui->setupUi(this);
    carsDialog = new CarsDialog(this);
    connect(ui->buyCarBtn, &QPushButton::clicked, carsDialog, &QDialog::show);
    connect(carsDialog, &CarsDialog::requestBuy, this, &PlayerCarWidget::handleBuy);
    connect(&carInfoMsg, &TCPMsg::responseReady, this, [](const QJsonObject& response){
        if(response["status"]=="success") {
            qDebug()<<response;
        } else {qDebug()<<"Problem z autem";}
    });
    handleNoCar();
    connect(&buyCarMsg, &TCPMsg::responseReady, this, [this](const QJsonObject& jsonobj ){
        if(jsonobj.contains("model")) {
            if(jsonobj["name"]=="points") {
                QMessageBox* msgBox = new QMessageBox(this);
                msgBox->setWindowFlag(Qt::FramelessWindowHint,true);
                msgBox->setWindowOpacity(0.96);
                msgBox->setText("Posiadasz niewystarczającą ilość punktów, by kupić ten pojazd.");
                QPushButton* okBtn = new QPushButton("Ok");
                msgBox->addButton(okBtn, QMessageBox::AcceptRole);
                msgBox->setAttribute(Qt::WA_DeleteOnClose);
                msgBox->open();
            } else if(jsonobj.contains("model")) {
                handleCar(jsonobj["name"].toString(), jsonobj["model"].toInt());
                emit refreshUserData(this->login);
            }
        }
    });
}

void PlayerCarWidget::logedIn(const QString& login) {
    QJsonObject obj;
    this->login = login;
    obj.insert("login", login);
    carInfoMsg.send(obj);
}

void PlayerCarWidget::handleNoCar() {
    ui->carModelWidget->hide();
    ui->widget_2->hide();
    ui->tuningBtn->hide();
}

void PlayerCarWidget::handleCar(const QString& name, unsigned int model, const QStringList& partsOwned, unsigned short colorId) {
    ui->carModelWidget->show();
    ui->widget_2->show();
    ui->tuningBtn->show();
    ui->carModelWidget->setStyleSheet("#carModelWidget {border-image:url(:/cars/cars/"+name+".jpg);}");
    ui->modelLabel->setText(name);
    ui->tuningLabel->setText(QString::number(partsOwned.count()));
    ui->colorButton->setStyleSheet("background-color: #"+QString::number(vehicleColors[colorId],16)+";");
}

PlayerCarWidget::~PlayerCarWidget()
{
    delete ui;
}

void PlayerCarWidget::handleBuy(const QString &name, unsigned int model, unsigned int price)
{
    QJsonObject obj;
    obj.insert("login", login);
    obj.insert("name", name);
    obj.insert("model", QString::number(model));
    obj.insert("price", QString::number(price));
    buyCarMsg.send(obj);
}
