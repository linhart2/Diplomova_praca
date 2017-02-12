<?php

namespace App\Presenters;

use Nette;
use App\Model\ArticleManager;



class HomepagePresenter extends Nette\Application\UI\Presenter
{
    /** @var ArticleManager */
    private $articleManager;

    public function __construct(ArticleManager $articleManager)
    {
        $this->articleManager = $articleManager;
    }

    public function renderDefault()
    {
        $this->template->posts = $this->articleManager->getPublicArticles()->limit(5);
        //$all_users = $this->articleManager->findAll()->orderBy('id')->fetchAll();
        //$this->template->all_users = $all_users;
    }
    public function handleUsersJson() {
        $all_users = array();
        $all_users["aaData"] = $this->articleManager->findAll()->orderBy('id')->fetchAll();
        $this->sendResponse(new Nette\Application\Responses\JsonResponse($all_users));
    }

}