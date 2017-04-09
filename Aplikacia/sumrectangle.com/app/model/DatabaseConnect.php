<?php

namespace App\Model;

use Nette;
use Nette\Security as NS;


class DatabaseConnect
{
	use Nette\SmartObject;

    private $database;

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

    public function addClass($data) {

        return $this->database->table('classes')->insert([
            'teacher_id' => $id,
            'class_name' => $data->class_name,
        ]);

    }

}
