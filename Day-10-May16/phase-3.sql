/*
	Phase 3: SQL Joins Practice
	
	Write queries to:
	
	1. List students and the courses they enrolled in
	2. Show students who received certificates with trainer names
	3. Count number of students per course
*/

-- 1. List students and the courses they enrolled in
-- students, courses, enrollments

SELECT s.name as student_name, c.course_name
FROM Students s LEFT JOIN Enrollments e
ON e.student_id = s.student_id
LEFT JOIN Courses c on e.course_id = c.course_id;

-- 2. Show students who received certificates with trainer names
-- students, enrollments, coursetrainers, trainers, certificates

SELECT s.name as student_name,
	t.trainer_name,
	c.course_name, 
	ce.serial_no
FROM 
	Certificates ce
JOIN
	Enrollments e ON ce.enrollment_id = e.enrollment_id
JOIN
	Students s on e.student_id = s.student_id
JOIN
	Courses c on e.course_id = c.course_id
JOIN
	CourseTrainers ct on c.course_id = ct.course_id
JOIN
	Trainers t on ct.trainer_id = t.trainer_id;

-- To list all enrollment info
SELECT
	s.name as student_name,
	t.trainer_name,
	c.course_name,
	ce.serial_no,
	CASE WHEN ce.certificate_id IS NULL THEN 'NO' ELSE 'YES' END AS has_certificate
FROM 
	Enrollments e
JOIN
	Students s ON e.student_id = s.student_id
JOIN
	Courses c ON e.course_id = c.course_id
JOIN
	CourseTrainers ct ON c.course_id = ct.course_id
JOIN
	Trainers t ON ct.trainer_id = t.trainer_id
LEFT JOIN
	Certificates ce ON e.enrollment_id = ce.enrollment_id;

--- 	3. Count number of students per course
SELECT
	c.course_name,
	COUNT(e.student_id) AS students_count
FROM
	Courses c
LEFT JOIN 
	Enrollments e ON c.course_id = e.course_id
LEFT JOIN
	Students s on e.student_id = s.student_id
GROUP BY
	c.course_name;