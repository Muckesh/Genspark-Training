/*
	Phase 5: Cursor
	
	Use a cursor to:
	
	* Loop through all students in a course
	* Print name and email of those who do not yet have certificates

*/

DO $$
DECLARE
	student_record RECORD;
	student_cursor CURSOR FOR
		SELECT s.name, s.email
		FROM Students s
		JOIN Enrollments e on s.student_id = e.student_id
		LEFT JOIN Certificates ce on e.enrollment_id = ce.enrollment_id
		WHERE ce.certificate_id IS NULL;
BEGIN
	OPEN student_cursor;

	LOOP
		FETCH student_cursor INTO student_record;
		EXIT WHEN NOT FOUND;

		RAISE NOTICE 'Student Name : %, Email : %.',student_record.name,student_record.email;
	END LOOP;
	CLOSE student_cursor;
END;
$$

select * from students;

select * from enrollments;