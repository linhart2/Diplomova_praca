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

    public function findAll() {
        return $this->database->select('id, email, name, role')->from($this->table);
    }
    public function register($data) {        
        unset($data["password2"]);
        $data["role"] = "guest";
        $data["country"] = "slovakia";
        $data["password"] = NS\Passwords::hash($data["password"]);
        return $this->database->table('teachers')->insert([
	        'first_name' => $data->first_name,
            'last_name' => $data->last_name,
	        'mail' => $data->mail,
	        'password' => $data->password,
	        'zip' => $data->zip,
            'country' => $data->country,
	    ]);
        
    }

}
