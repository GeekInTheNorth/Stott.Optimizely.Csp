import React, { useState, useEffect } from 'react';
import { Card, Container } from 'react-bootstrap';
import EditCrossOriginHeaders from './EditCrossOriginHeaders';
import EditHeaderSettings from './EditHeaderSettings';
import EditStrictTransportSecurity from './EditStrictTransportSecurity';
import axios from 'axios';

function SecurityHeaderContainer(props) {
    
    const [isXctoHeaderEnabled, setIsXctoHeaderEnabled] = useState(false);
    const [isXfoHeaderEnabled, setIsXfoHeaderEnabled] = useState('None');
    const [isXxpHeaderEnabled, setIsXxpHeaderEnabled] = useState('None');
    const [isRpHeaderEnabled, setIsRpHeaderEnabled] = useState('None');
    const [crossOriginEmbedderPolicy, setCrossOriginEmbedderPolicy] = useState('None');
    const [crossOriginOpenerPolicy, setCrossOriginOpenerPolicy] = useState('None');
    const [crossOriginResourcePolicy, setCrossOriginResourcePolicy] = useState('None');
    const [isStrictTransportHeaderEnabled, setIsStrictTransportHeaderEnabled] = useState(false);
    const [isIncludeSubDomainsChecked, setIsIncludeSubDomainsChecked] = useState(false);
    const [maxAgeParameter, setMaxAgeParameter] = useState(63072000);
    const [isMounted, setIsMounted] = useState(false);

    useEffect(() => {
        getCspSettings()
    }, [])
    
    const getCspSettings = async () => {
        const response = await axios.get(process.env.REACT_APP_SECURITY_HEADER_GET_URL)
        setIsStrictTransportHeaderEnabled(response.data.isStrictTransportSecurityEnabled);
        setIsIncludeSubDomainsChecked(response.data.isStrictTransportSecuritySubDomainsEnabled);
        setMaxAgeParameter(response.data.strictTransportSecurityMaxAge);
        setCrossOriginEmbedderPolicy(response.data.crossOriginEmbedderPolicy);
        setCrossOriginOpenerPolicy(response.data.crossOriginOpenerPolicy);
        setCrossOriginResourcePolicy(response.data.crossOriginResourcePolicy);
        setIsXctoHeaderEnabled(response.data.xContentTypeOptions);
        setIsXxpHeaderEnabled(response.data.xXssProtection);
        setIsXfoHeaderEnabled(response.data.xFrameOptions);
        setIsRpHeaderEnabled(response.data.referrerPolicy);
        setIsMounted(true);
    }

    return(
        <>
            { isMounted ?
            <Container fluid='md'>
                <Card className='mb-3'>
                    <Card.Header className='bg-primary text-light'>General Security Headers</Card.Header>
                    <Card.Body>
                        <EditHeaderSettings isXctoHeaderEnabled={isXctoHeaderEnabled}
                                            isXfoHeaderEnabled={isXfoHeaderEnabled}
                                            isXxpHeaderEnabled={isXxpHeaderEnabled}
                                            isRpHeaderEnabled={isRpHeaderEnabled}
                                            showToastNotificationEvent={props.showToastNotificationEvent}></EditHeaderSettings>
                    </Card.Body>
                </Card>
                <Card className='mb-3'>
                    <Card.Header className='bg-primary text-light'>Cross Origin Headers</Card.Header>
                    <Card.Body>
                        <EditCrossOriginHeaders crossOriginEmbedderPolicy={crossOriginEmbedderPolicy}
                                                crossOriginOpenerPolicy={crossOriginOpenerPolicy}
                                                crossOriginResourcePolicy={crossOriginResourcePolicy}
                                                showToastNotificationEvent={props.showToastNotificationEvent}></EditCrossOriginHeaders>
                    </Card.Body>
                </Card>
                <Card className='mb-3'>
                    <Card.Header className='bg-primary text-light'>HTTP Strict Transport Security Header (HSTS)</Card.Header>
                    <Card.Body>
                        <EditStrictTransportSecurity isStrictTransportHeaderEnabled={isStrictTransportHeaderEnabled}
                                                     isIncludeSubDomainsChecked={isIncludeSubDomainsChecked}
                                                     maxAgeParameter={maxAgeParameter}
                                                     showToastNotificationEvent={props.showToastNotificationEvent}></EditStrictTransportSecurity>
                    </Card.Body>
                </Card>
            </Container>
            : null }
        </>
    )
}

export default SecurityHeaderContainer