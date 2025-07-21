
export interface TrainingVideo{
    id: string
    title: string
    description: string
    uploadDate: Date
    blobUrl: string
}

export interface UploadTrainingVideo{
    title: string
    description: string
    video: File
}