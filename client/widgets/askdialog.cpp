#include "askdialog.h"
#include "ui_askdialog.h"

AskDialog::AskDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::AskDialog)
{
    ui->setupUi(this);
}

AskDialog::~AskDialog()
{
    delete ui;
}
