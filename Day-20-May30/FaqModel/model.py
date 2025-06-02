from fastapi import FastAPI
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer, util
import torch
import json

app = FastAPI()

model = SentenceTransformer("all-MiniLM-L6-v2")

with open("faq_data.json","r",encoding="utf-8") as f:
    faq_data = json.load(f)

questions = [item["question"] for item in faq_data]

answers = [item["answer"] for item in faq_data]

question_embeddings = model.encode(questions, convert_to_tensor=True)


class Query(BaseModel):
    question: str

@app.post("/ask")
def ask_question(query: Query):
    user_embedding = model.encode(query.question, convert_to_tensor=True)

    similarities = util.pytorch_cos_sim(user_embedding, question_embeddings)
 
    index = torch.argmax(similarities).item()

    confidence = similarities[0][index].item()

    return {
        "answer": answers[index], 
        "confidence": round(confidence, 3)
        }
