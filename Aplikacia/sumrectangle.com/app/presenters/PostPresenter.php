<?php
namespace App\Presenters;

use Nette;
use Nette\Application\UI\Form;


class PostPresenter extends Nette\Application\UI\Presenter
{
	    /** @var Nette\Database\Context */
    private $database;
    public function __construct(Nette\Database\Context $database)
	{
		$this->database = $database;
	}

	public function renderShow($postId)
	{
	    $post = $this->database->table('posts')->get($postId);
	    if (!$post) {
	        $this->error('Stránka nebyla nalezena');
	    }

	    $this->template->post = $post;
	    $this->template->comments = $post->related('comment')->order('created_at');
	}
	protected function createComponentCommentForm()
	{
	    $form = new Form; // means Nette\Application\UI\Form

	    $form->addText('name', 'Jméno:')
	        ->setRequired();

	    $form->addEmail('email', 'Email:');

	    $form->addTextArea('content', 'Komentář:')
	        ->setRequired();

	    $form->addSubmit('send', 'Publikovat komentář');
	    $form->onSuccess[] = [$this, 'commentFormSucceeded'];
	    return $form;
	}
	public function commentFormSucceeded($form, $values)
	{
	    $postId = $this->getParameter('postId');

	    $this->database->table('comments')->insert([
	        'post_id' => $postId,
	        'name' => $values->name,
	        'email' => $values->email,
	        'content' => $values->content,
	    ]);

	    $this->flashMessage('Děkuji za komentář', 'success');
	    $this->redirect('this');
	}
	protected function createComponentPostForm()
	{
	    $form = new Form;
	    $form->addText('title', 'Titulek:')
	        ->setRequired();
	    $form->addTextArea('content', 'Obsah:')
	        ->setRequired();

	    $form->addSubmit('send', 'Uložit a publikovat');
	    $form->onSuccess[] = [$this, 'postFormSucceeded'];

	    return $form;
	}
	public function postFormSucceeded($form, $values)
	{
		if (!$this->getUser()->isLoggedIn()) {
        	$this->error('Pro vytvoření, nebo editování příspěvku se musíte přihlásit.');
    	}
	    $postId = $this->getParameter('postId');

	    if ($postId) {
	        $post = $this->database->table('posts')->get($postId);
	        $post->update($values);
	    } else {
	        $post = $this->database->table('posts')->insert($values);
	    }

	    $this->flashMessage('Příspěvek byl úspěšně publikován.', 'success');
	    $this->redirect('show', $post->id);
	}
	public function actionEdit($postId)
    {
    	if (!$this->getUser()->isLoggedIn()) {
        	$this->redirect('Sign:in');
    	}
	    $post = $this->database->table('posts')->get($postId);
	    if (!$post) {
	        $this->error('Příspěvek nebyl nalezen');
	    }
	    $this['postForm']->setDefaults($post->toArray());
	}
	public function actionCreate()
	{
	    if (!$this->getUser()->isLoggedIn()) {
	        $this->redirect('Sign:in');
	    }
	}
}
