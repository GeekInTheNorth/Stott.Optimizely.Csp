import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import PermissionModal from './PermissionModal';

function AddPermission(props) {
    
    const [showCreateModal, setShowCreateModal] = useState(false);
    
    const handleReloadSources = () => props.reloadSourceEvent();
    const handleCloseCreateModal = () => setShowCreateModal(false);
    const handleShowCreateModal = () => setShowCreateModal(true);

    return (
        <>
            <Button variant='success' className='fw-bold' onClick={handleShowCreateModal}>+</Button>
            {showCreateModal ? <PermissionModal show={showCreateModal} id='00000000-0000-0000-0000-000000000000' source='' directives='default-src' reloadSourceEvent={handleReloadSources} closeModalEvent={handleCloseCreateModal}></PermissionModal> : null}
        </>
    )
}

export default AddPermission