/*
	Phase 6: Security & Roles

*/
/*
	1. Create a `readonly_user` role:
	
	   * Can run `SELECT` on `students`, `courses`, and `certificates`
	   * Cannot `INSERT`, `UPDATE`, or `DELETE`
	   
*/
-- creating role
CREATE ROLE readonly_user LOGIN PASSWORD 'readonly@123';

-- revoking all default access
REVOKE ALL ON Students, Courses, Certificates FROM readonly_user;

-- granting only select previleges
GRANT SELECT ON Students, Courses, Certificates TO readonly_user;

/*
	2. Create a `data_entry_user` role:
	
	   * Can `INSERT` into `students`, `enrollments`
	   * Cannot modify certificates directly
*/

-- creating role
CREATE ROLE data_entry_user LOGIN PASSWORD 'dataentry@123';

-- revoking all default access
REVOKE ALL ON Students, Enrollments, Certificates FROM data_entry_user;

-- granting insert previleges
GRANT SELECT, INSERT ON Students, Enrollments TO data_entry_user;