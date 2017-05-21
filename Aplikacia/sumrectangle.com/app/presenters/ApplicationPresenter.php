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
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
            setcookie("teacherID", $this->getUser()->getId());
            $this->template->posts = $this->databaseConnect->getClasses();
        }
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
        if($post){
            $this->flashMessage('Trieda bola pridana!', 'success');
            $this->redirect('Application:showclass');
        }else{
            $this->flashMessage('Niekde sa to pokazilo!', 'error');
        }
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
            $this->flashMessage('Trieda bola odstranena!', 'info');
            $this->redirect('Application:showclass');
        }
    }
    public function renderAddclass()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }
    public function renderAddexam()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }
    public function renderEditclass()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }
    public function renderEditexam()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }
    public function renderOpenexam()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }

    public function renderShowexam()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }
    public function renderShowexams()
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
        }
    }



    public function renderShowstudent($classId,$className)
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
            $this->session->classId = $this->getParameter('classId');
            $this->template->className = $className;
            $this->template->posts = $this->databaseConnect->getStudentIntoClass($classId);
        }
    }
    public function renderOpenstudent($studentId, $studentFirstName, $studentLastName)
    {
        if (!$this->getUser()->isLoggedIn()) {
            $this->redirect('Sign:in');
        }else{
            $this->template->studentFirstName = $studentFirstName;
            $this->template->studentLastName = $studentLastName;
            $this->template->posts = $this->databaseConnect->getStudentIntoClass($studentId);
        }
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
        $form->addText('exam_name', 'Nazov cvicenia');

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