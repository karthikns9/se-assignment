import React, { useEffect, useMemo, useState } from "react";
import ReactSelect from "react-select";
import { upsertAssignments } from "../../../api/api";

const PlanProcedureItem = ({ procedure, users, planId, initialAssignedUserIds }) => {
    console.log('users', users);
    const [selectedUsers, setSelectedUsers] = useState(null);
    const usersLookup = useMemo(() => Object.fromEntries(users.map(u => [u.value, u])), [users]);
    console.log('userOptionsById', usersLookup);

    useEffect(() => {
        if (!initialAssignedUserIds || initialAssignedUserIds.length === 0) {
            return;
        }
        const mapped = initialAssignedUserIds
            .map(id => usersLookup[id])
            .filter(Boolean);
        if ((selectedUsers === null || selectedUsers?.length === 0) && mapped.length > 0) {
            setSelectedUsers(mapped);
        }
        console.log('mapped', mapped);
    }, [initialAssignedUserIds, usersLookup, selectedUsers]);

    const handleAssignUserToProcedure = async (e) => {
        console.log('e', e);
        setSelectedUsers(e);
        const ids = Array.isArray(e) ? e.map(x => x.value) : [];
        try {
            await upsertAssignments(Number(planId), Number(procedure.procedureId), ids);
        } catch (err) {
            console.error(err);
        }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;