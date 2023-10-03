#include "skindialog.h"
#include "ui_skindialog.h"

#include <QDirIterator>
#include <QLabel>
#include <QPushButton>

SkinDialog::SkinDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::SkinDialog)
{
    ui->setupUi(this);
    QDirIterator it(":/skins/normal/", QDirIterator::Subdirectories);
    int i=0;
    QWidget* horWidget{};
    while (it.hasNext()) {
        auto frame = new QFrame();
        frame->setLayout(new QVBoxLayout());
        frame->layout()->setContentsMargins(0,0,0,0);
        frame->setObjectName("frameButton");
        frame->setAccessibleName("frameButton");
        auto btn = new QPushButton();
        auto currentPath = it.next();
        btn->setStyleSheet("border-image:url("+currentPath+"); border:0;");
        auto stringId = currentPath.remove(":/skins/normal/").remove("Skin_").remove(".png");
        frame->layout()->addWidget(btn);
        btn->setMinimumSize(55,100);
        frame->setMinimumSize(59,104);
        btn->setMaximumSize(55,100);
        frame->setMinimumSize(59,104);
        bool valid = false;
        int id = stringId.toInt(&valid);
        if(valid) {
            connect(btn, &QPushButton::clicked, this, [id,this](){
                emit skinSelected(id);
                this->close();
            });
        }
        if(i%12==0) {
            horWidget = new QWidget();
            horWidget->setLayout(new QHBoxLayout());
            horWidget->layout()->setContentsMargins(0,0,0,0);
            ui->contentWidget->layout()->addWidget(horWidget);
        }
        horWidget->layout()->addWidget(frame);
        i++;
    }
}

SkinDialog::~SkinDialog()
{
    delete ui;
}
