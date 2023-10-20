QT       += core gui network

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

CONFIG += c++17

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
    carsdialog.cpp \
    carwidget.cpp \
    connection.cpp \
    main.cpp \
    mainwindow.cpp \
    playercarwidget.cpp \
    playerdata.cpp \
    session.cpp \
    skindialog.cpp \
    static/StylesheetLoader.cpp \
    widgets/askdialog.cpp

HEADERS += \
    carsdialog.h \
    carwidget.h \
    connection.h \
    mainwindow.h \
    playercarwidget.h \
    playerdata.h \
    session.h \
    skindialog.h \
    static/StylesheetLoader.h \
    widgets/askdialog.h

FORMS += \
    carsdialog.ui \
    carwidget.ui \
    connection.ui \
    mainwindow.ui \
    playercarwidget.ui \
    playerdata.ui \
    session.ui \
    skindialog.ui \
    widgets/askdialog.ui

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

RESOURCES += \
    data.qrc


CONFIG( release ) {
    DESTDIR = G:/Games/SAMP/partymode/deploy/
    QMAKE_POST_LINK = C:/Qt/6.5.2/msvc2019_64/bin/windeployqt $$shell_path($$DESTDIR/$${TARGET}.exe)
}

