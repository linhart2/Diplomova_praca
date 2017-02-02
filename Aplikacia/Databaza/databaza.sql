CREATE TABLE IF NOT EXISTS `teacher` (
	`user_id` int(10) unsigned DEFAULT NULL,
	`name` varchar(70) COLLATE utf8_unicode_ci DEFAULT NULL,
	`mail` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`passwd` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`class_id` int(10) unsigned DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `student` (
	`user_id` int(10) unsigned DEFAULT NULL,
	`name` varchar(70) COLLATE utf8_unicode_ci DEFAULT NULL,
	`result_id` int(10) unsigned DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `class_room` (
	`class_id` int(10) unsigned DEFAULT NULL,
	`students_id` int(10) unsigned DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

—- correct = id_exam 
CREATE TABLE IF NOT EXISTS `result` (
	`result_id` int(10) unsigned DEFAULT NULL,
	`correct` int(10) unsigned DEFAULT NULL,
	`wrong` int(10) unsigned DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;