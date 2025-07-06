const express = require('express');
const mongoose = require('mongoose');

const app = express();
const PORT = 3000;
const MONGO_URL = process.env.MONGO_URL || 'mongodb://localhost:27017/db';

mongoose.connect(MONGO_URL,{
    useNewUrlParser: true,
    useUnifiedTopology: true,
})
.then(()=>console.log("Connected to MongoDb"))
.catch(err=>console.error('MonogDb connection error : ',err));

app.get('/',(req,res)=>{
    res.send('Nodejs Api is running and connected to MongoDb');
});

app.listen(PORT,()=>{
    console.log(`Api running on port ${PORT}`);
});

