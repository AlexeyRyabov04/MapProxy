import React, { useState, useEffect } from 'react';
import axios from 'axios';


const App = () => {
  const [serviceName, setServiceName] = useState('');
    const [maxRequests, setMaxRequests] = useState('');
    const [accessRules, setAccessRules] = useState([]);
    const [loading, setLoading] = useState(true);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!serviceName || !maxRequests) {
            alert("Please fill in all fields");
            return;
        }

        const accessRule = {
            ServiceName: serviceName,
            MaxRequests: parseInt(maxRequests, 10),
        };

        try {
            await axios.post('http://localhost:5092/api/AccessRules', accessRule);
            setServiceName('');
            setMaxRequests('');
            fetchAccessRules(); 
        } catch (error) {
            console.error('Error sending access rule:', error);
        }
    };

    const fetchAccessRules = async () => {
        try {
            const response = await axios.get('http://localhost:5092/api/Statistics');
            setAccessRules(response.data);
            
        } catch (error) {
            console.error('Error fetching access rules:', error);
        }
        finally {
          setLoading(false);
      }
    };

    useEffect(() => {
        fetchAccessRules();
    }, []);
    if (loading) return <p>Loading...</p>;
    return (
        <div style={{ padding: '20px' }}>
            <h1>Access Rule Management</h1>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '10px' }}>
                    <input
                        type="text"
                        placeholder="Service Name"
                        value={serviceName}
                        onChange={(e) => setServiceName(e.target.value)}
                        required
                    />
                    <input
                        type="number"
                        placeholder="Max Requests"
                        value={maxRequests}
                        onChange={(e) => setMaxRequests(e.target.value)}
                        required
                    />
                    <button type="submit">Submit</button>
                </div>
            </form>
            <h2>Access Rules Table</h2>
            <table>
                <thead>
                    <tr>
                        <th>Service Name</th>
                        <th>Client IP</th>
                        <th>Remaining Requests</th>
                        <th>Processed Requests</th>
                    </tr>
                </thead>
                <tbody>
                    {accessRules.map((rule, index) => (
                        <tr key={index}>
                            <td>{rule.serviceName}</td>
                            <td>{rule.clientIp}</td>
                            <td>{rule.remainingRequests}</td>
                            <td>{rule.processedRequests}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default App;
