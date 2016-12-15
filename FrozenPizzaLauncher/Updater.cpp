#include "Updater.hh"

Updater::Updater() : QWidget()

{
  QVBoxLayout *VLayout = new QVBoxLayout(this);

  setFixedSize(400,600);
  setWindowTitle("FrozenPizza Launcher");
  version = "1.0";
  label.setText("<strong>Installed version:</strong>" + version);
  updateBtn = new QPushButton("Check for Updates", this);
  VLayout->addWidget(&label);
  VLayout->addWidget(updateSoftware);
  QObject::connect(updateBtn, SIGNAL(clicked()), this, SLOT(startUpdate()));
}

void Updater::startUpdate()
{
  QNetworkAccessManager manager;
  QEventLoop loop;
  string latestVersion;

  reply = manager.get(QNetworkRequest(QUrl("http://rs4family.fr/frozenpizza/version.cfg"))); // Url vers le fichier version.txt
  QObject::connect(reply, SIGNAL(finished()), &loop, SLOT(quit()));
  updateSoftware->setEnabled(false);
  loop.exec();
  versionNew = reply->readAll();
  updateBtn->setEnabled(true);
  if (version != latestVersion)
    {
      QMessageBox::information(this,"Update Info", "There is an available update!");
      close();
    }
  else
    {
      QMessageBox::information(this,"Update Info", "FrozenPizza is up to date!");

    }

}
