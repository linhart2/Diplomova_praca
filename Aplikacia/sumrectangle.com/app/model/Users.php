<?php
class Users extends \Nette\Object {
		/** @var \DibiConnection */
	    private $db;
	        private $table = "users";
	        public static $user_salt = "AEcx199opQ";
		    public function __construct(\DibiConnection $connection) {
			            $this->db = $connection;
				        }
		    public function findAll() {
			            return $this->db->select('id, email, name, role')->from($this->table);
				        }
		    public function register($data) {        
			            unset($data["password2"]);
				            $data["role"] = "guest";
				            $data["password"] = sha1($data["password"] . self::$user_salt);
					            return $this->db->insert($this->table, $data)->execute(dibi::IDENTIFIER);
					        }
}
