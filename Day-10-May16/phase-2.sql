/*

Phase 2: DDL & DML

* Create all tables with appropriate constraints (PK, FK, UNIQUE, NOT NULL)
* Insert sample data using `INSERT` statements
* Create indexes on `student_id`, `email`, and `course_id`

*/
-------------------- DDL --------------------------------
CREATE TABLE Students (
	student_id SERIAL PRIMARY KEY,
	name TEXT NOT NULL,
	email TEXT UNIQUE NOT NULL,
	phone VARCHAR(15) UNIQUE NOT NULL	
)

drop table students;

CREATE TABLE Courses (
	course_id SERIAL PRIMARY KEY,
	course_name TEXT NOT NULL,
	category TEXT NOT NULL,
	duration_days INT NOT NULL
)

drop table courses;

CREATE TABLE Trainers (
	trainer_id SERIAL PRIMARY KEY,
	trainer_name TEXT NOT NULL,
	expertise TEXT NOT NULL
)

drop table trainers;

CREATE TABLE Enrollments (
	enrollment_id SERIAL PRIMARY KEY,
	student_id INT NOT NULL REFERENCES Students(student_id) ON DELETE CASCADE,
	course_id INT NOT NULL REFERENCES Courses(course_id) ON DELETE CASCADE,
	enroll_date DATE NOT NULL,
	UNIQUE(student_id, course_id)
)

drop table enrollments;

CREATE TABLE Certificates (
	certificate_id SERIAL PRIMARY KEY,
	enrollment_id INT NOT NULL REFERENCES Enrollments(enrollment_id),
	issued_date DATE NOT NULL,
	serial_no TEXT UNIQUE NOT NULL 
)

drop table certificates;

CREATE TABLE CourseTrainers (
	course_id INT NOT NULL REFERENCES Courses(course_id),
	trainer_id INT NOT NULL REFERENCES Trainers(trainer_id),
	UNIQUE(course_id,trainer_id)
)

drop table coursetrainers;
---------------------------------------------------------------------
---------------- DML -------------------------------------
----------- Students ------------------------
INSERT INTO Students (name,email,phone) VALUES
('Alice Johnson', 'alice@example.com', '9876543210'),
('Bob Smith', 'bob@example.com', '9876543211'),
('Charlie Roy', 'charlie@example.com', '9876543212'),
('Diana Paul', 'diana@example.com', '9876543213'),
('Evan Thomas', 'evan@example.com', '9876543214'),
('Fiona Shah', 'fiona@example.com', '9876543215'),
('George Mathew', 'george@example.com', '9876543216'),
('Hannah Lee', 'hannah@example.com', '9876543217'),
('Ian Wright', 'ian@example.com', '9876543218'),
('Judy Kim', 'judy@example.com', '9876543219'),
('Kevin Brown', 'kevin@example.com', '9876543220'),
('Laura Green', 'laura@example.com', '9876543221'),
('Mike Davis', 'mike@example.com', '9876543222'),
('Nina Patel', 'nina@example.com', '9876543223'),
('Oscar Lin', 'oscar@example.com', '9876543224');

INSERT INTO Students (name,email,phone) VALUES
('Tom Cruise','tomc@example.com','9876543267'),
('Tom Hiddleston','tomhd@example.com','9876543967');

SELECT * FROM Students;

TRUNCATE TABLE Students CASCADE;

------------- Courses ------------------------------
INSERT INTO Courses (course_name,category,duration_days) VALUES
('Python Basics', 'Programming', 30),
('Data Science', 'Analytics', 60),
('Web Development', 'Frontend', 45),
('Java for Beginners', 'Programming', 40),
('Machine Learning', 'AI', 90);

INSERT INTO Courses (course_name,category,duration_days) VALUES
('C# Basics', 'Programming', 120);

SELECT * FROM Courses;

TRUNCATE TABLE Courses CASCADE;

-------------- Trainers -------------------------------------
INSERT INTO Trainers (trainer_name,expertise) VALUES
('David Kumar', 'Python, Java'),
('Rina Das', 'Data Science, ML'),
('Arjun Mehta', 'Frontend'),
('Sara Fernandes', 'AI, ML'),
('Mohit Jain', 'Full Stack');

SELECT * FROM Trainers;

TRUNCATE TABLE Trainers CASCADE;


----------------- Course Trainers ---------------------
INSERT INTO CourseTrainers (course_id, trainer_id) VALUES
(1, 1),  -- Python Basics → David
(2, 2),  -- Data Science → Rina
(3, 3),  -- Web Dev → Arjun
(4, 1),  -- Java → David
(5, 4);  -- ML → Sara

SELECT * FROM CourseTrainers;

TRUNCATE TABLE CourseTrainers;

----------------- Enrollments ---------------------
INSERT INTO Enrollments (student_id,course_id,enroll_date) VALUES
(1, 1, '2025-01-01'), -- Alice → Python
(2, 1, '2025-01-02'), -- Bob → Python
(3, 2, '2025-01-03'), -- Charlie → Data Science
(4, 3, '2025-01-04'), -- Diana → Web Dev
(5, 4, '2025-01-05'), -- Evan → Java
(6, 5, '2025-01-06'), -- Fiona → ML
(7, 1, '2025-01-07'), -- George → Python
(8, 2, '2025-01-08'), -- Hannah → Data Science
(9, 3, '2025-01-09'), -- Ian → Web Dev
(10, 5, '2025-01-10'), -- Judy → ML
(11, 4, '2025-01-11'), -- Kevin → Java
(12, 3, '2025-01-12'), -- Laura → Web Dev
(13, 1, '2025-01-13'), -- Mike → Python
(14, 2, '2025-01-14'), -- Nina → Data Science
(15, 5, '2025-01-15'); -- Oscar → ML

SELECT * FROM Enrollments;

TRUNCATE TABLE Enrollments CASCADE;

--------------- Certificates -----------------------
INSERT INTO Certificates (enrollment_id, issued_date, serial_no) VALUES
(1, '2025-02-01', 'CERT-PY-0001'),  -- Alice
(3, '2025-02-05', 'CERT-DS-0002'),  -- Charlie
(4, '2025-02-06', 'CERT-WEB-0003'), -- Diana
(6, '2025-02-07', 'CERT-ML-0004'),  -- Fiona
(10, '2025-02-10', 'CERT-ML-0005'), -- Judy
(12, '2025-02-12', 'CERT-WEB-0006'); -- Laura

SELECT * FROM Certificates;

TRUNCATE TABLE Certificates;

----------------------------- Indexing ---------------------------
-- student_id
CREATE INDEX idx_students_id ON Students(student_id);

-- email
CREATE INDEX idx_students_email ON Students(email);

-- course_id
CREATE INDEX idx_courses_id ON Courses(course_id);