import { TagModel } from "../ApiClient/Main";


export interface ITagService {
    getAllUsedTags(): Promise<TagModel[]>;
}
