<?php

namespace App\Model;

use Nette;
use Nette\Security as NS;

class ArticleManager
{
	use Nette\SmartObject;

    private $database;
    private $table = "users";
    public static $user_salt = "AEcx199opQ";

    public function __construct(Nette\Database\Context $database)
	{
	    $this->database = $database;
    }

    public function getPublicArticles()
    {
        return $this->database->table('posts')
        ->where('created_at < ', new \DateTime())
        ->order('created_at DESC');
    }

    public function findAll() {
        return $this->database->select('id, email, name, role')->from($this->table);
    }
    public function register($data) {        
        unset($data["password2"]);
        $data["role"] = "guest";
        $data["password"] = NS\Passwords::hash($data["password"]);
        return $this->database->table('users')->insert([
	        'name' => $data->name,
	        'email' => $data->email,
	        'password' => $data->password,
	        'role' => $data->role,
	    ]);
        
    }

}
