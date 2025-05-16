/*
	You are tasked with building a PostgreSQL-backed database for an EdTech company that manages online training and certification programs for individuals across various technologies.

	The goal is to:
	
	Design a normalized schema
	
	Support querying of training data
	
	Ensure secure access
	
	Maintain data integrity and control over transactional updates
	
	Database planning (Nomalized till 3NF)
	
	A student can enroll in multiple courses
	
	Each course is led by one trainer
	
	Students can receive a certificate after passing
	
	Each certificate has a unique serial number
	
	Trainers may teach multiple courses
*/

database EdTech
--- my answer
1. student
   student_id, name, email, phone, register_Date, is_active

2. trainer
  trainer_id, name, email, phone, hire_Date, is_Active

-- each course led by one trainer
	-> one to many relationship, so no separate trainerCourse Table required.
courses
  course_id, name, duration, price, startdate, enddate, trainer_id, is_active

-- student can enroll in many courses and single course can have multiple student
	-> many-to-many relationship so separate studentcourse table is required.
studentCourse
   student_id, course_id, enrolldate

cerificate
    certificate_id,serial_no, student_id, course_id, issued_Date
------------ actual answer --------------------------
Tables to Design (Normalized to 3NF):

1. **students**

   * `student_id (PK)`, `name`, `email`, `phone`

2. **courses**

   * `course_id (PK)`, `course_name`, `category`, `duration_days`

3. **trainers**

   * `trainer_id (PK)`, `trainer_name`, `expertise`

4. **enrollments**

   * `enrollment_id (PK)`, `student_id (FK)`, `course_id (FK)`, `enroll_date`

5. **certificates**

   * `certificate_id (PK)`, `enrollment_id (FK)`, `issue_date`, `serial_no`

6. **course\_trainers** (Many-to-Many if needed)

   * `course_id`, `trainer_id`

---
