CREATE TABLE IF NOT EXISTS `teachers` (
	`teacher_id` int(10) unsigned NOT NULL,
	`first_name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`last_name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`mail` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`password` varchar(60) COLLATE utf8_unicode_ci DEFAULT NULL,
	`zip` int(10) unsigned DEFAULT NULL,
	`country` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
	`school` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `students` (
	`student_id` int(10) unsigned NOT NULL,
	`first_name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`last_name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`mail` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`password` varchar(60) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `classes` (
	`class_id` int(10) unsigned NOT NULL,
	`teacher_id` int(10) unsigned DEFAULT NULL,
	`class_name` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
	`class_passwd` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `class` (
	`id` int(10) unsigned NOT NULL,
	`student_id` int(10) unsigned DEFAULT NULL,
	`class_id` int(10) unsigned DEFAULT NULL,
	`result` boolean not null default 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `exams` (
	`exam_id` int(10) unsigned NOT NULL,
	`teacher_id` int(10) unsigned DEFAULT NULL,
	`exam_string` text COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE IF NOT EXISTS `results` (
	`results_id` int(10) unsigned NOT NULL,
	`example_id` int(10) unsigned DEFAULT NULL,
	`student_id` int(10) unsigned DEFAULT NULL,
	`result` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

ALTER TABLE `teachers`
	ADD PRIMARY KEY (`mail`),
	ADD KEY (`teacher_id`);
ALTER TABLE `teachers`
	MODIFY `teacher_id` int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=1;

ALTER TABLE `students`
	ADD PRIMARY KEY (`mail`),
	ADD KEY (`student_id`);
ALTER TABLE `students`
	MODIFY `student_id` int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=1;

ALTER TABLE `classes`
	ADD PRIMARY KEY (`class_id`);
ALTER TABLE `classes`
	MODIFY `class_id` int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=1;

ALTER TABLE `class`
	ADD PRIMARY KEY (`id`);
ALTER TABLE `class`
	MODIFY `id` int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=1;

ALTER TABLE `exams`
	ADD PRIMARY KEY (`exam_id`);
ALTER TABLE `exams`
	MODIFY `exam_id` int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=1;


ALTER TABLE `classes`
	ADD CONSTRAINT `Fk_classes_teacher` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`teacher_id`) ON DELETE CASCADE;
ALTER TABLE `class`
	ADD CONSTRAINT `Fk_class_student` FOREIGN KEY (`student_id`) REFERENCES `students` (`student_id`) ON DELETE CASCADE;
	
ALTER TABLE `class`
	ADD CONSTRAINT `Fk_class_classes` FOREIGN KEY (`class_id`) REFERENCES `classes` (`class_id`) ON DELETE CASCADE;


/* 
ALTER TABLE `moderators`
	ADD KEY `user_id` (`user_id`);
  
ALTER TABLE `forums`
	ADD PRIMARY KEY (`forum_id`);
  
ALTER TABLE `topics`
	ADD PRIMARY KEY (`topic_id`),
	ADD KEY `forum_id` (`forum_id`),
	ADD KEY `user_id` (`user_id`);
  
ALTER TABLE `comments`
	ADD PRIMARY KEY (`comment_id`),
	ADD KEY `topic_id` (`topic_id`),
	ADD KEY `user_id` (`user_id`);
*/