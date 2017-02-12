<?php

namespace App\Presenters;

use Nette;
use Nette\Application\UI\Form;
use App\Model\ArticleManager;


class RegisterPresenter extends Nette\Application\UI\Presenter {

    /** @var Users */
    private $articleManager;

    public function __construct(ArticleManager $articleManager)
    {
        $this->articleManager = $articleManager;
    }

    public function renderRegister(){
    }


    protected function createComponentRegisterForm() {
        $form = new Form;
        $form->addText('name', 'Jméno');
        $form->addText('email', 'E-mail: *', 35)
                ->setEmptyValue('@')
                ->addRule(Form::FILLED, 'Vyplňte Váš email')
                ->addCondition(Form::FILLED)
                ->addRule(Form::EMAIL, 'Neplatná emailová adresa');
        $form->addPassword('password', 'Heslo: *', 20)
                ->setOption('description', 'Alespoň 6 znaků')
                ->addRule(Form::FILLED, 'Vyplňte Vaše heslo')
                ->addRule(Form::MIN_LENGTH, 'Heslo musí mít alespoň %d znaků.', 6);
        $form->addPassword('password2', 'Heslo znovu: *', 20)
                ->addConditionOn($form['password'], Form::VALID)
                ->addRule(Form::FILLED, 'Heslo znovu')
                ->addRule(Form::EQUAL, 'Hesla se neshodují.', $form['password']);
        $form->addSubmit('send', 'Registrovat');
        $form->onSuccess[] = [$this, 'registerFormSubmitted'];
        return $form;
    }

    public function registerFormSubmitted($form) 
    {
	    $values = $form->getValues();
	    $new_user_id = $this->articleManager->register($values);
	    if($new_user_id){
	        $this->flashMessage('Registrace se zdařila, jo!');
	        $this->redirect('Sign:in');
	    }
	}

}