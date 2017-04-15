<?php

namespace App\Model;

use Nette;
use Nette\Security as NS;



class DatabaseConnect extends Nette\Application\UI\Presenter
{
	use Nette\SmartObject;

    private $database;


    public function __construct(Nette\Database\Context $database)
	{
	    $this->database = $database;
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

    public function getClasses()
    {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('classes')
                ->where('teacher_id = ', $this->getUser()->getId());
        }
        $this->error($this->getUser()->getId());
    }
    public function getStudentIntoClass($id)
    {
        if ($this->getUser()->isLoggedIn()) {

            return $this->database->query('SELECT students.student_id, students.first_name, students.last_name, class.class_id
FROM students
INNER JOIN class
ON class.student_id=students.student_id
WHERE class_id = ?', $id);
        }
        $this->error($this->getUser()->getId());
    }

    public function addClass($data) {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('classes')->insert([
                'teacher_id' => $this->getUser()->getId(),
                'class_name' => $data->class_name,
                'class_passwd' => $data->class_passwd,
            ]);
        }
        $this->error($this->getUser()->getId());

    }
    public function editClass($data) {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('classes')->get($data);
        }
        $this->error($this->getUser()->getId());
    }
    public function removeClass($data) {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('classes')->where('class_id = ', $data)->delete();
        }
        $this->error($this->getUser()->getId());
    }

    public function removeStudent($data) {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('class')->where('student_id = ', $data)->delete();
        }
        $this->error($this->getUser()->getId());
    }

    public function addExam($data) {
        if ($this->getUser()->isLoggedIn()) {
            return $this->database->table('classes')->insert([
                'teacher_id' => $this->getSession()->getId(),
                'class_name' => $data->class_name,
            ]);
        }
        $this->error($this->getUser()->getId());
    }
}
