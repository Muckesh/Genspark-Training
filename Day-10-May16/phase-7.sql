/*
	Phase 7: Transactions & Atomicity
	
	Write a transaction block that:
	
	* Enrolls a student
	* Issues a certificate
	* Fails if certificate generation fails (rollback)
	
	```sql
	BEGIN;
	-- insert into enrollments
	-- insert into certificates
	-- COMMIT or ROLLBACK on error
	```
*/

CREATE OR REPLACE PROCEDURE sp_enroll_and_certify(
	p_student_id INT,
	p_course_id INT
) LANGUAGE plpgsql
AS $$
DECLARE
	v_enrollment_id int;
BEGIN
	-- start transaction
	BEGIN
		-- insert into enrollments
		INSERT INTO Enrollments (student_id,course_id,enroll_date)
		VALUES (p_student_id,p_course_id,current_date)
		RETURNING enrollment_id into v_enrollment_id;

		-- insert into certificates
		INSERT INTO Certificates (enrollment_id,issued_date)
		VALUES (v_enrollment_id,current_date);

		Raise NOTICE 'Student is Enrolled and Certified';
		EXCEPTION
		WHEN OTHERS THEN
			-- if there are any errors rollback transaction
			ROLLBACK;
			RAISE NOTICE 'Transaction Failed and was rolled back : %.',SQLERRM;
	END;
	
END;
$$;

CALL sp_enroll_and_certify(1, 2);

SELECT * FROM Enrollments;
SELECT * FROM Certificates;












DO $$
DECLARE
	v_enrollment_id INT;
BEGIN
	-- start transaction
	BEGIN
		-- insert into enrollments
		INSERT INTO Enrollments (student_id,course_id,enroll_date)
		VALUES (1,3,current_date)
		RETURNING enrollment_id into v_enrollment_id;

		-- insert into certificates
		INSERT INTO Certificates (enrollment_id,issued_date)
		VALUES (v_enrollment_id,current_date);

		-- commit transaction
		
	EXCEPTION
		WHEN OTHERS THEN
			-- if there are any errors rollback transaction
			ROLLBACK;
			RAISE NOTICE 'Transaction Failed and was rolled back : %.',SQLERRM;
	END;
END;
$$


