import logo from './logo.svg';
import './App.css';
import { useEffect, useState } from 'react';

function App() {
  const [message,setMessage]=useState('');

  useEffect(()=>{
    // fetch('http://backend:5000/hello')
    fetch('/hello')
    .then(res=>res.json())
    .then(data=>setMessage(data.message))
    .catch(err=> console.error(err));
  },[]);

  return (
    <div>
      <h1>Frontend App</h1>
      <p>{message}</p>
    </div>
  );
}

export default App;
