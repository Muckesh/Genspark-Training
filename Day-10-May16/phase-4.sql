/*
	Phase 4: Functions & Stored Procedures

	Function:
	
	Create `get_certified_students(course_id INT)`
	→ Returns a list of students who completed the given course and received certificates.
	
	Stored Procedure:
	
	Create `sp_enroll_student(p_student_id, p_course_id)`
	→ Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).

*/

-- 	Create `get_certified_students(course_id INT)`


CREATE OR REPLACE FUNCTION get_certified_students(
	p_course_id INT
)
RETURNS TABLE (
	student_name TEXT,
	email TEXT,
	phone VARCHAR(15),
	course_name TEXT,
	trainer_name TEXT,
	certificate_serial_no TEXT
)
AS $$
BEGIN
	RETURN Query
	SELECT
		s.name,s.email,s.phone, c.course_name,t.trainer_name, ce.serial_no
	From Certificates ce 
	JOIN Enrollments e ON ce.enrollment_id = e.enrollment_id
	JOIN students s ON e.student_id = s.student_id
	JOIN courses c ON e.course_id = c.course_id
	JOIN coursetrainers ct ON c.course_id = ct.course_id
	JOIN trainers t ON ct.trainer_id = t.trainer_id
	WHERE c.course_id = p_course_id;
END;
$$ LANGUAGE PLPGSQL;

DROP FUNCTION get_certified_students(integer)

select * from get_certified_students(3);

-- 	Create `sp_enroll_student(p_student_id, p_course_id)`
select * from enrollments;
select * from students;
select * from courses;
CREATE OR REPLACE PROCEDURE sp_enroll_student (
	p_student_id int,
	p_course_id int,
	p_is_completed boolean
)
LANGUAGE Plpgsql
AS $$
declare
	v_enrollment_id int;
begin
	begin
	insert into enrollments(student_id,course_id,enroll_date) values
	(p_student_id,p_course_id,current_date)
	returning enrollment_id into v_enrollment_id;

	if p_is_completed then
	insert into Certificates(enrollment_id,issued_date) values
	(v_enrollment_id,current_date);
	end if;
	
	end;
end;
$$;

call sp_enroll_student(3,5,true);


--- trigger to auto generate serial no
-- trigger fn
create or replace function generate_certificate_serial()
returns trigger as $$
declare
	course_abbr text;
begin
	select UPPER(SUBSTRING(c.course_name,1,3)) into course_abbr
	from courses c
	join enrollments e on c.course_id = e.course_id
	where e.enrollment_id = new.enrollment_id;
    -- Generate serial number in format: CERT-{ABBR}-{SEQ}
	new.serial_no := 'CERT-' || course_abbr || '-' ||
		lpad(new.certificate_id :: TEXT,6,'0');

	return new;
end;
$$ language plpgsql;

drop trigger trg_generate_serial on certificates
---- creating trigger
create trigger trg_generate_serial
before insert on Certificates
for each row
execute function generate_certificate_serial();

select * from certificates;
