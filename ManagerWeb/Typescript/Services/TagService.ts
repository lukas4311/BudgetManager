import { TagApiInterface } from "../ApiClient/Main";
import { ITagService } from "./ITagService";

export default class TagService implements ITagService {
    private tagApi: TagApiInterface;

    constructor(tagApi: TagApiInterface) {
        this.tagApi = tagApi;
    }

    public async getAllUsedTags() {
        const tags = await this.tagApi.tagsAllUsedGet();
        return tags;
    }
}