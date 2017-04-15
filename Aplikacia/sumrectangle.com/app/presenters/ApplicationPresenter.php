<?php

namespace App\Presenters;

use Nette;
use App\Model\DatabaseConnect;
use Nette\Application\UI\Form;
use Nette\Http\Session;




class ApplicationPresenter extends Nette\Application\UI\Presenter
{
    private $databaseConnect;
    private $session;

    public function __construct(DatabaseConnect $databaseConnect, Nette\Http\Session $session)
    {
        $this->databaseConnect = $databaseConnect;
        $this->session = $session->getSection('classId');
    }

    public function renderShowclass()
    {
        $this->template->posts = $this->databaseConnect->getClasses();
    }
    protected function createComponentClassForm() {
        $form = new Form;
        $form->addText('class_name', 'Meno triedy')
            ->addRule(Form::FILLED, 'Vyplňte meno triedy');
        $form->addText('class_passwd', 'Kód pre vstup do triedy')
            ->addRule(Form::FILLED, 'Vyplňte kód triedy');
        $form->addSubmit('send', 'Pridať');
        $form->onSuccess[] = [$this, 'classFormSucceeded'];
        return $form;
    }
    public function classFormSucceeded($form, $values)
    {
        $classId = $this->getParameter('classId');
        if ($classId) {
            $post = $this->databaseConnect->editClass($classId);
            $post->update($values);
        } else {
            $post = $this->databaseConnect->addClass($values);
        }

        $this->flashMessage('Trieda bola pridana!', 'success');
        $this->redirect('Application:showclass');
    }
    public function actionEditclass($classId)
    {
        $post = $this->databaseConnect->editClass($classId);
        if (!$post) {
            $this->error('Příspěvek nebyl nalezen');
        }
        $this['classForm']->setDefaults($post->toArray());
    }
    public function actionDeleteclass($classId)
    {
        $post = $this->databaseConnect->removeClass($classId);
        if($post){
            $this->flashMessage('Trieda bola odstranena!', 'success');
            $this->redirect('Application:showclass');
        }
    }

    public function renderShowstudent($classId)
    {
        $this->session->classId = $this->getParameter('classId');
        $this->template->posts = $this->databaseConnect->getStudentIntoClass($classId);
    }
    public function renderOpenstudent($studentId)
    {
        $this->template->posts = $this->databaseConnect->getStudentIntoClass($studentId);
    }
    public function actionDeletestudent($studentId)
    {
        $post = $this->databaseConnect->removeStudent($studentId);
        if($post){
            $this->flashMessage('Ziak bol odstraneny z triedy!', 'success');
            $this->redirect('Application:showstudent', $this->session->classId);
        }
    }


    protected function createComponentExamForm() {
        $form = new Form;
        $form->addText('first_name', 'Meno');
        $form->addText('last_name', 'Priezvisko');
        $form->addText('mail', 'E-mail: *', 35)
            ->setEmptyValue('@')
            ->addRule(Form::FILLED, 'Vyplňte Váš email')
            ->addCondition(Form::FILLED)
            ->addRule(Form::EMAIL, 'Neplatná emailová adresa');
        $form->addPassword('password', 'Heslo: *', 20)
            ->setOption('description', 'Aspoň 6 znakov')
            ->addRule(Form::FILLED, 'Vyplňte Vaše heslo')
            ->addRule(Form::MIN_LENGTH, 'Heslo musí mať aspoň %d znakov.', 6);
        $form->addPassword('password2', 'Heslo znovu: *', 20)
            ->addConditionOn($form['password'], Form::VALID)
            ->addRule(Form::FILLED, 'Heslo znovu')
            ->addRule(Form::EQUAL, 'Heslá sa nezhodujú.', $form['password']);
        $form->addText('zip', 'PSČ:')
            ->setRequired()
            ->addRule(Form::PATTERN, 'PSČ musí mít 5 číslic', '([0-9]\s*){5}');
        $form->addSelect('country', 'Země:')
            ->setPrompt('Zvolte zemi');
        $form->addSubmit('send', 'Registrovať');
        $form->onSuccess[] = [$this, 'registerFormSubmitted'];
        return $form;
    }
    public function examFormSucceeded($form, $values)
    {
        $postId = $this->getParameter('postId');
        if ($postId) {
            $post = $this->databaseConnect->editClass($postId);
            $post->update($values);
        } else {
            $post = $this->databaseConnect->addClass($values);
        }

        $this->flashMessage('Trieda bola pridana!', 'success');
        $this->redirect('Application:showclass');
    }


}