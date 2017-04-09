<?php
use Nette\Security as NS;
use Tracy\Debugger;
Debugger::enable(); 

class Authenticator extends Nette\Object implements NS\IAuthenticator
{
    public $database;

    function __construct(Nette\Database\Context $database)
    {
        $this->database = $database;
    }

    function authenticate(array $credentials)
    {
        list($username, $password) = $credentials;
        $row = $this->database->table('teachers')
            ->where('mail', $username)->fetch();

        if (!$row) {
            throw new NS\AuthenticationException('User not found.');
        }

        if (!NS\Passwords::verify($password, $row->password)) {
            throw new NS\AuthenticationException('Invalid password.');
        }
        return new NS\Identity($row->teacher_id, array('first_name' => $row->first_name, 'last_name' => $row->last_name, 'teacher_id' => $row->teacher_id));
    }
}